// PDF generation in the browser using jsPDF. Called from Blazor via MathHelpPdf.download(json).
// Layout: 2 columns of groups, 5 rows of groups = 10 groups per page, 10 problems per group = 100 per page.
// Portrait A4. Aligned ×, = and drawn answer line (no underscores).
(function () {
    'use strict';

    var PROBLEMS_PER_GROUP = 10;
    var GROUPS_PER_PAGE = 10;
    var COLS_PER_ROW = 2;
    var PROBLEMS_PER_PAGE = 100;
    var PAGE_W = 210;
    var PAGE_H = 297;
    var MARGIN = 15;
    var TITLE_MARGIN_TOP = 18;
    var LINE_HEIGHT = 5.2;
    var FONT_SIZE = 10;
    var ANSWER_LINE_LENGTH = 18;

    function getJsPdfLib() {
        var lib = (typeof jspdf !== 'undefined') ? jspdf : (typeof window.jspdf !== 'undefined' ? window.jspdf : null);
        if (lib && lib.jsPDF) return lib;
        throw new Error('jsPDF not loaded');
    }

    function getJsPdf() {
        return new getJsPdfLib().jsPDF();
    }

    function drawProblemRow(doc, lib, groupX, groupY, groupW, globalIndex, a, b, answer, showAnswer, useFormFields, y) {
        doc.setFontSize(FONT_SIZE);
        var timesStr = ' × ';
        var equalsStr = ' = ';
        var wTimes = doc.getTextWidth(timesStr);
        var wEquals = doc.getTextWidth(equalsStr);
        var maxAW = doc.getTextWidth('12');
        var maxBW = doc.getTextWidth('12');
        var aStr = String(a);
        var bStr = String(b);
        var xTimes = groupX + maxAW;
        var xEquals = xTimes + wTimes + maxBW;
        var xLineStart = xEquals + wEquals;
        var xLineEnd = Math.min(xLineStart + ANSWER_LINE_LENGTH, groupX + groupW - 2);

        doc.text(aStr, xTimes - doc.getTextWidth(aStr), y);
        doc.text(timesStr, xTimes, y);
        doc.text(bStr, xEquals - doc.getTextWidth(bStr), y);
        doc.text(equalsStr, xEquals, y);
        if (showAnswer) {
            doc.text(String(answer), xLineStart, y);
        } else if (useFormFields && lib && lib.AcroForm && typeof lib.AcroForm.TextField === 'function' && typeof doc.addField === 'function') {
            try {
                var TextField = lib.AcroForm.TextField;
                var textField = new TextField();
                textField.fieldName = 'ans_' + String(globalIndex);
                textField.Rect = [xLineStart, y - 3.5, Math.min(ANSWER_LINE_LENGTH, groupX + groupW - 2 - xLineStart), 4.5];
                doc.addField(textField);
            } catch (e) {
                doc.setLineWidth(0.2);
                doc.line(xLineStart, y + 0.8, xLineEnd, y + 0.8);
            }
        } else {
            doc.setLineWidth(0.2);
            doc.line(xLineStart, y + 0.8, xLineEnd, y + 0.8);
        }
    }

    function renderPageOfGroups(doc, lib, problems, title, showAnswers, problemOffset, useFormFields) {
        doc.setFontSize(14);
        doc.text(title, PAGE_W / 2, TITLE_MARGIN_TOP, { align: 'center' });
        doc.setFontSize(FONT_SIZE);

        var contentTop = TITLE_MARGIN_TOP + 10;
        var groupW = (PAGE_W - 2 * MARGIN) / COLS_PER_ROW;
        var groupH = (PAGE_H - contentTop - MARGIN) / 5;
        var col0X = MARGIN;
        var col1X = MARGIN + groupW;

        for (var g = 0; g < GROUPS_PER_PAGE; g++) {
            var col = g % COLS_PER_ROW;
            var row = Math.floor(g / COLS_PER_ROW);
            var groupX = col === 0 ? col0X : col1X;
            var groupY = contentTop + row * groupH;
            doc.setFontSize(9);
            doc.setTextColor(100, 100, 100);
            doc.text('Group ' + (g + 1), groupX, groupY + 1.5);
            doc.setTextColor(0, 0, 0);
            doc.setFontSize(FONT_SIZE);
            for (var i = 0; i < PROBLEMS_PER_GROUP; i++) {
                var idx = g * PROBLEMS_PER_GROUP + i;
                if (idx >= problems.length) break;
                var p = problems[idx];
                var y = groupY + 3 + i * LINE_HEIGHT;
                var globalIndex = problemOffset + idx;
                drawProblemRow(doc, lib, groupX, groupY, groupW, globalIndex, p.a, p.b, p.ans, showAnswers, useFormFields, y);
            }
        }
    }

    function downloadTablePdf(opts) {
        var lib = getJsPdfLib();
        var doc = new lib.jsPDF();
        var minTable = opts.minTable;
        var maxTable = opts.maxTable;
        var showAnswers = opts.showAnswers;
        var problems = opts.problems || [];
        var pageNum = 0;
        for (var start = 0; start < problems.length; start += PROBLEMS_PER_PAGE) {
            if (pageNum > 0) doc.addPage();
            var chunk = problems.slice(start, start + PROBLEMS_PER_PAGE);
            var title = 'Multiplication tables ' + minTable + '-' + maxTable;
            if (problems.length > PROBLEMS_PER_PAGE) {
                title += ' (page ' + (pageNum + 1) + ')';
            }
            renderPageOfGroups(doc, lib, chunk, title, showAnswers, start, false);
            pageNum++;
        }
        doc.save(showAnswers ? 'multiplication-table-answers.pdf' : 'multiplication-table.pdf');
    }

    function downloadPracticePdf(opts) {
        var lib = getJsPdfLib();
        var doc = new lib.jsPDF();
        var sheets = opts.sheets || [];
        for (var s = 0; s < sheets.length; s++) {
            if (s > 0) doc.addPage();
            var problems = sheets[s];
            var title = 'Practice sheet' + (sheets.length > 1 ? ' ' + (s + 1) : '');
            renderPageOfGroups(doc, lib, problems, title, false, s * PROBLEMS_PER_PAGE, true);
        }
        var name = sheets.length > 1 ? 'practice-sheets-' + sheets.length + '.pdf' : 'practice-sheet.pdf';
        doc.save(name);
    }

    window.MathHelpPdf = {
        download: function (jsonString) {
            try {
                var opts = JSON.parse(jsonString);
                if (opts.type === 'table') {
                    downloadTablePdf(opts);
                } else if (opts.type === 'practice') {
                    downloadPracticePdf(opts);
                }
            } catch (e) {
                console.error('MathHelpPdf.download failed', e);
            }
        }
    };
})();

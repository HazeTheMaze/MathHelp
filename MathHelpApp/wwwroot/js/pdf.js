// PDF generation in the browser using jsPDF. Called from Blazor via MathHelpPdf.download(json).
// Layout: 3 columns of groups, 10 problems per group; fixed-width groups; partial last row centered.
// Portrait A4. Answer line always drawn for print.
(function () {
    'use strict';

    var PROBLEMS_PER_GROUP = 10;
    var GROUPS_PER_PAGE = 10;
    var COLS = 3;
    var PROBLEMS_PER_PAGE = 100;
    /** Max groups per page for reference (table) PDFs: 4 cols × 3 rows = 12. */
    var REFERENCE_GROUPS_PER_PAGE = 12;
    var PAGE_W = 210;
    var PAGE_H = 297;
    var MARGIN = 15;
    var TITLE_MARGIN_TOP = 18;
    var TITLE_RULE_OFFSET = 4;
    var TITLE_CONTENT_GAP = 8;
    var LINE_HEIGHT = 5.2;
    var GROUP_VSPACING = 8;
    var FONT_SIZE = 10;
    var ANSWER_LINE_LENGTH = 18;
    var FOOTER_HEIGHT = 12;
    var FOOTER_MARGIN_BOTTOM = 10;
    var FOOTER_FONT_SIZE_NAME = 9;
    var FOOTER_FONT_SIZE_URL = 8;

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
        // Center the whole problem (a × b = ____) horizontally within the group
        var totalContentWidth = maxAW + wTimes + maxBW + wEquals + ANSWER_LINE_LENGTH;
        var contentStartX = groupX + (groupW - totalContentWidth) / 2;
        var xTimes = contentStartX + maxAW;
        var xEquals = xTimes + wTimes + maxBW;
        var xLineStart = xEquals + wEquals;
        var xLineEnd = Math.min(xLineStart + ANSWER_LINE_LENGTH, groupX + groupW - 2);

        doc.text(aStr, xTimes - doc.getTextWidth(aStr), y);
        doc.text(timesStr, xTimes, y);
        doc.text(bStr, xEquals - doc.getTextWidth(bStr), y);
        doc.text(equalsStr, xEquals, y);
        if (showAnswer) {
            doc.text(String(answer), xLineStart, y);
        } else {
            doc.setLineWidth(0.2);
            doc.line(xLineStart, y + 0.8, xLineEnd, y + 0.8);
            if (useFormFields && lib && lib.AcroForm && typeof lib.AcroForm.TextField === 'function' && typeof doc.addField === 'function') {
                try {
                    var TextField = lib.AcroForm.TextField;
                    var textField = new TextField();
                    textField.fieldName = 'ans_' + String(globalIndex);
                    textField.Rect = [xLineStart, y - 3.5, Math.min(ANSWER_LINE_LENGTH, groupX + groupW - 2 - xLineStart), 4.5];
                    doc.addField(textField);
                } catch (e) { /* line already drawn */ }
            }
        }
    }

    function drawFooter(doc, siteName, siteUrl) {
        if (!siteName && !siteUrl) return;
        var y = PAGE_H - FOOTER_MARGIN_BOTTOM;
        doc.setDrawColor(200, 200, 200);
        doc.setLineWidth(0.2);
        doc.line(MARGIN, y - 5, PAGE_W - MARGIN, y - 5);
        doc.setDrawColor(0, 0, 0);
        if (siteName) {
            doc.setFont('helvetica', 'bold');
            doc.setFontSize(FOOTER_FONT_SIZE_NAME);
            doc.text(siteName, PAGE_W / 2, y - 1, { align: 'center' });
        }
        if (siteUrl) {
            doc.setFont('helvetica', 'normal');
            doc.setFontSize(FOOTER_FONT_SIZE_URL);
            doc.text(siteUrl, PAGE_W / 2, y + 4, { align: 'center' });
        }
    }

    function renderPageOfGroups(doc, lib, problems, title, showAnswers, problemOffset, useFormFields, siteName, siteUrl, problemsPerGroup, cols) {
        var perGroup = (problemsPerGroup !== undefined && problemsPerGroup !== null) ? problemsPerGroup : PROBLEMS_PER_GROUP;
        var colsUsed = (cols !== undefined && cols !== null) ? cols : COLS;

        doc.setFont('times', 'bold');
        doc.setFontSize(16);
        doc.text(title, PAGE_W / 2, TITLE_MARGIN_TOP, { align: 'center' });
        doc.setLineWidth(0.3);
        doc.line(MARGIN, TITLE_MARGIN_TOP + TITLE_RULE_OFFSET, PAGE_W - MARGIN, TITLE_MARGIN_TOP + TITLE_RULE_OFFSET);
        doc.setFont('helvetica', 'normal');
        doc.setFontSize(FONT_SIZE);

        var contentTop = TITLE_MARGIN_TOP + TITLE_RULE_OFFSET + TITLE_CONTENT_GAP;
        var contentWidth = PAGE_W - 2 * MARGIN;
        var contentBottom = PAGE_H - MARGIN - FOOTER_HEIGHT;
        var colW = contentWidth / colsUsed;
        var groupHeight = perGroup * LINE_HEIGHT + GROUP_VSPACING;

        var numGroups = Math.ceil(problems.length / perGroup);
        var totalRows = Math.ceil(numGroups / colsUsed);

        for (var g = 0; g < numGroups; g++) {
            var col = g % colsUsed;
            var row = Math.floor(g / colsUsed);
            // Last row may have fewer groups: center that row horizontally; full rows stay left-aligned
            var numGroupsInRow = (row === totalRows - 1)
                ? (numGroups - row * colsUsed)
                : colsUsed;
            var rowStartX = MARGIN + (contentWidth - numGroupsInRow * colW) / 2;
            var groupX = rowStartX + col * colW;
            var groupY = contentTop + row * groupHeight;

            for (var i = 0; i < perGroup; i++) {
                var idx = g * perGroup + i;
                if (idx >= problems.length) break;
                var p = problems[idx];
                var y = groupY + i * LINE_HEIGHT;
                var globalIndex = problemOffset + idx;
                drawProblemRow(doc, lib, groupX, groupY, colW, globalIndex, p.a, p.b, p.ans, showAnswers, useFormFields, y);
            }
        }

        drawFooter(doc, siteName, siteUrl);
    }

    function getSiteOpts(opts) {
        return {
            siteName: opts.siteName || 'MathHelp',
            siteUrl: opts.siteUrl || ''
        };
    }

    function downloadTablePdf(opts) {
        var lib = getJsPdfLib();
        var doc = new lib.jsPDF();
        var site = getSiteOpts(opts);
        var minTable = opts.minTable;
        var maxTable = opts.maxTable;
        var showAnswers = opts.showAnswers;
        var problemsPerGroup = opts.problemsPerGroup;
        var problems = opts.problems || [];
        // Reference PDFs always use 4 columns. Paginate by full groups only.
        var cols = 4;
        var problemsPerPage = problemsPerGroup * REFERENCE_GROUPS_PER_PAGE;
        var pageNum = 0;
        var titleRangeFormat = opts.pdfTitleMultiplicationTables || 'Multiplication tables {0}-{1}';
        var titlePageSuffixFormat = opts.pdfTitlePageSuffix || ' (page {0})';
        for (var start = 0; start < problems.length; start += problemsPerPage) {
            if (pageNum > 0) doc.addPage();
            var chunk = problems.slice(start, start + problemsPerPage);
            var title = titleRangeFormat.replace('{0}', minTable).replace('{1}', maxTable);
            if (problems.length > problemsPerPage) {
                title += titlePageSuffixFormat.replace('{0}', pageNum + 1);
            }
            renderPageOfGroups(doc, lib, chunk, title, showAnswers, start, false, site.siteName, site.siteUrl, problemsPerGroup, cols);
            pageNum++;
        }
        doc.save(showAnswers ? 'multiplication-table-answers.pdf' : 'multiplication-table.pdf');
    }

    function downloadPracticePdf(opts) {
        var lib = getJsPdfLib();
        var doc = new lib.jsPDF();
        var site = getSiteOpts(opts);
        var sheets = opts.sheets || [];
        var showAnswers = opts.showAnswers === true;
        var practiceLabel = opts.pdfPracticeSheet || 'Practice sheet';
        var answerLabel = opts.pdfAnswerSheet || 'Answer sheet';
        var sheetNumberSuffix = opts.pdfSheetNumberSuffix !== undefined ? opts.pdfSheetNumberSuffix : ' {0}';
        for (var s = 0; s < sheets.length; s++) {
            if (s > 0) doc.addPage();
            var problems = sheets[s];
            var title = (showAnswers ? answerLabel : practiceLabel) + (sheets.length > 1 ? sheetNumberSuffix.replace('{0}', s + 1) : '');
            renderPageOfGroups(doc, lib, problems, title, showAnswers, s * PROBLEMS_PER_PAGE, !showAnswers, site.siteName, site.siteUrl, PROBLEMS_PER_GROUP);
        }
        var name = sheets.length > 1
            ? (showAnswers ? 'practice-sheets-' + sheets.length + '-answers.pdf' : 'practice-sheets-' + sheets.length + '.pdf')
            : (showAnswers ? 'practice-sheet-answers.pdf' : 'practice-sheet.pdf');
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

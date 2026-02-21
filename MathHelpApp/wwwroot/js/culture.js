window.MathHelpCulture = {
    get: function () {
        return localStorage.getItem('mathhelp-culture') || '';
    },
    set: function (culture) {
        if (culture === 'sv' || culture === 'en') {
            localStorage.setItem('mathhelp-culture', culture);
        } else {
            localStorage.removeItem('mathhelp-culture');
        }
    },
    getInitial: function () {
        var qs = new URLSearchParams(window.location.search);
        var fromUrl = qs.get('culture');
        if (fromUrl === 'sv' || fromUrl === 'en') return fromUrl;
        var stored = localStorage.getItem('mathhelp-culture');
        if (stored === 'sv' || stored === 'en') return stored;
        return 'en';
    },
    setLang: function (culture) {
        if (culture === 'sv' || culture === 'en') {
            document.documentElement.lang = culture;
        }
    }
};

// Sync URL with stored culture so ?culture= is present when user had a preference (runs after this script has loaded).
(function () {
    var stored = window.MathHelpCulture.get();
    if (stored === 'sv' || stored === 'en') {
        var qs = new URLSearchParams(window.location.search);
        if (qs.get('culture') !== stored) {
            qs.set('culture', stored);
            var url = window.location.pathname + '?' + qs.toString() + window.location.hash;
            window.location.replace(url);
        }
    }
})();

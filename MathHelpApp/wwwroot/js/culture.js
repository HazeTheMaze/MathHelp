window.MathHelpCulture = {
    get: function () {
        return localStorage.getItem('mathhelp-culture') || '';
    },
    set: function (culture) {
        localStorage.setItem('mathhelp-culture', culture);
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

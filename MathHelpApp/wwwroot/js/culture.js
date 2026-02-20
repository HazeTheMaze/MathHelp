window.MathHelpCulture = {
    get: function () {
        return localStorage.getItem('mathhelp-culture') || '';
    },
    set: function (culture) {
        localStorage.setItem('mathhelp-culture', culture);
    },
    /** Returns culture from URL ?culture= or from localStorage (so Blazor can set culture before first render). */
    getInitial: function () {
        var qs = new URLSearchParams(window.location.search);
        var fromUrl = qs.get('culture');
        if (fromUrl === 'sv' || fromUrl === 'en') return fromUrl;
        var stored = localStorage.getItem('mathhelp-culture');
        if (stored === 'sv' || stored === 'en') return stored;
        return 'en';
    }
};

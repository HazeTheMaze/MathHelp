window.MathHelpDropdown = {
    // Registers a click-outside handler for a dropdown component
    registerClickOutside: function (dotNetHelper, elementSelector) {
        const element = document.querySelector(elementSelector);
        if (!element) return;

        const handleClickOutside = function (event) {
            // Check if the click is outside the element
            if (!element.contains(event.target)) {
                dotNetHelper.invokeMethodAsync('CloseFromJavaScript');
            }
        };

        // Store the handler so we can remove it later
        element._clickOutsideHandler = handleClickOutside;
        document.addEventListener('click', handleClickOutside);
    },

    // Unregisters the click-outside handler for a dropdown component
    unregisterClickOutside: function (elementSelector) {
        const element = document.querySelector(elementSelector);
        if (!element || !element._clickOutsideHandler) return;

        document.removeEventListener('click', element._clickOutsideHandler);
        delete element._clickOutsideHandler;
    }
};

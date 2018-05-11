(function ($, ko) {

    // Binding handler for loading button plugin
    ko.bindingHandlers.modalOpen = {
        init: function (element, valueAccessor, allBindings, viewModel) {

            // jQueryify the element
            var $el = $(element);

            // Listen to changes to the observable
            ko.computed(function () {
                // Access the observable
                var isOpen = valueAccessor();

                if (isOpen()) {
                    $el.modal('show');
                    isOpen(true);

                } else {
                    $el.modal('hide');
                    isOpen(false);
                }

            }, null, { disposeWhenNodeIsRemoved: element });


        }
    };

})(jQuery, ko)
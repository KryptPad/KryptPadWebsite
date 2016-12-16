(function ($, ko) {

    // Binding handler for loading button plugin
    ko.bindingHandlers.modalOpen = {
        init: function (element, valueAccessor, allBindings, viewModel) {

            // jQueryify the element
            var $el = $(element);

            // Listen to changes to the observable
            ko.computed(function () {
                debugger
                // Access the observable
                var isOpen = ko.unwrap(valueAccessor());

                if (isOpen) {
                    $el.modal('show');

                } else {
                    $el.modal('hide');
                }

            }, null, { disposeWhenNodeIsRemoved: element });


        }
    };

})(jQuery, ko)
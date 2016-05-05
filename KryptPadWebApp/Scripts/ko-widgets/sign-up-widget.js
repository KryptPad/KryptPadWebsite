ko.components.register('sign-up-widget', {
    viewModel: function (params) {
        var self = this;

        self.isBusy = ko.observable(false);
        self.errorMessage = ko.observable();

        self.email = ko.observable();
        self.password = ko.observable();
        self.confirmPassword = ko.observable();


        self.signup = function () { };
        
    },
    template: { fromUrl: 'sign-up-widget.html' }
});

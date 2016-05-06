ko.components.register('reset-password-widget', {
    viewModel: function (params, model) {
        var self = this;

        // Store the data we got passed
        self.code = params.data.code;

        // Define some observables
        self.isBusy = ko.observable(false);
        self.errorMessage = ko.observable();
        self.email = ko.observable();
        self.password = ko.observable();
        self.confirmPassword = ko.observable();

        // Behaviors
        self.reset = function () {
            // Send all the data we need to the api to reset the password
            var postData = {
                email: self.email(),
                code: self.code,
                password: self.password(),
                confirmPassword: self.confirmPassword()
            };
            
            $.ajax({
                type: 'POST',
                url: '/api/account/reset-password',
                data: postData
            }).done(function (data) {
                // Success
                self.errorMessage(null);

            }).fail(function (error) {
                // Failed
                app.processError(error, function (message) {
                    // Show the error somewhere
                    self.errorMessage(message);
                });

            }).always(function () {
                // Set busy state
                self.isBusy(false);
            });
        };

    },
    template: { fromUrl: 'reset-password-widget.html' }
});

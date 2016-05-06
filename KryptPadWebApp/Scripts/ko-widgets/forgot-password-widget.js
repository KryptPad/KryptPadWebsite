ko.components.register('forgot-password-widget', {
    viewModel: function (params) {
        var self = this;

        self.email = ko.observable();
        self.isBusy = ko.observable(false);
        self.errorMessage = ko.observable();

        // User forgot password
        self.sendLink = function () {
            // Set busy state
            self.isBusy(true);

            var postData = {
                email: self.email()
            };

            $.ajax({
                type: 'POST',
                url: '/api/account/forgot-password',
                data: postData
            }).done(function (data) {
                // Success
                self.errorMessage(null);
                // Go back to login
                window.location.hash = "login";

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
    template: { fromUrl: 'forgot-password-widget.html' }
});

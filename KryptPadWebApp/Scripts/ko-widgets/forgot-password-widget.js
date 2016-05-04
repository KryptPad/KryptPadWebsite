ko.components.register('forgot-password-widget', {
    viewModel: function (params) {
        var self = this;

        self.username = ko.observable();
        self.isBusy = ko.observable(false);
        self.errorMessage = ko.observable();

        // User forgot password
        self.sendLink = function () {
            // Set busy state
            self.isBusy(true);

            var postData = {
                email: self.username()
            };

            $.ajax({
                type: 'POST',
                url: '/api/account/forgotpassword',
                data: postData
            }).done(function (data) {
                // Success

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

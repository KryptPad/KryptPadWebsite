ko.components.register('forgot-password-widget', {
    viewModel: function (params) {
        var self = this;

        self.email = ko.observable();
        self.isBusy = ko.observable(false);
        self.message = ko.observable();
        self.linkSent = ko.observable(false);

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
                self.message(app.createMessage(app.MSG_SUCCESS, "If the email address you provided is associated to your account, you should receive an email shortly with instructions on how to change your password."));
                
                // Set the flag
                self.linkSent(true);

            }).fail(function (error) {
                // Failed
                app.processError(error, function (message) {
                    // Show the error somewhere
                    self.message(app.createMessage(app.MSG_ERROR, message));
                });

            }).always(function () {
                // Set busy state
                self.isBusy(false);
                
            });
        };
    },
    template: { fromUrl: 'forgot-password-widget.html' }
});

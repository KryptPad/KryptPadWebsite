ko.components.register('confirm-email-widget', {
    viewModel: function (params) {
        var self = this;

        // Store the data we got passed
        self.userId = params.data.userId;
        self.code = params.data.code;

        // Define some observables
        self.isBusy = ko.observable(false);
        self.isSuccess = ko.observable(false);
        self.message = ko.observable();
        
        // Behaviors
        self.confirm = function () {
            
            self.isBusy(true);

            // Send all the data we need to the api to reset the password
            var postData = {
                userId: ko.unwrap(self.userId),
                code: ko.unwrap(self.code)
            };

            $.ajax({
                type: 'POST',
                url: '/api/account/confirm-email',
                data: postData
            }).done(function (data) {
                // Success
                self.isSuccess(true);
                // Show message
                self.message(app.createMessage(app.MSG_SUCCESS, 'Your email address has been confirmed successfully.'));

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

        // Trigger behaviors
        self.confirm();
    },
    template: { fromUrl: 'confirm-email-widget.html' }
});

ko.components.register('login-widget', {
    viewModel: function (params) {
        var self = this;

        self.username = ko.observable('ebutler@test.com');
        self.password = ko.observable();
        self.isAuthenticated = ko.observable(app.isAuthenticated());
        self.isBusy = ko.observable(false);
        self.errorMessage = ko.observable();

        

        self.login = function () {

            // Set busy state
            self.isBusy(true);

            var loginData = {
                grant_type: 'password',
                username: self.username(),
                password: self.password()
            };

            $.ajax({
                type: 'POST',
                url: '/token',
                data: loginData
            }).done(function (data) {
                // Cache the access token in session storage.
                app.setToken(data);
            
                // Attempt to login
                $('#login-form').submit();

            }).fail(function (error) {

                // Set busy state
                self.isBusy(false);

                app.processError(error, function (message) {
                    // Show the error somewhere
                    self.errorMessage(message);
                });

            }).complete(function () {
                // Leave busy state for login success
            });
        }
    },
    template: { fromUrl: 'login-widget.html' }
});

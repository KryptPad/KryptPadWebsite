ko.components.register('login-widget', {
    viewModel: function (params) {
        var self = this;

        self.username = ko.observable();
        self.password = ko.observable();
        self.isBusy = ko.observable(false);
        self.errorMessage = ko.observable();
                
        // Log in
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
            
                // Login successful, submit form for route handling. The route
                // will be picked up in the main-app.js file, and it will load
                // the profiles view
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

        // User forgot password
        self.forgotPassword = function () {
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
    template: { fromUrl: 'login-widget.html' }
});

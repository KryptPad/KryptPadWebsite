(function (global) {

    // Create Api object
    var api = global.api = global.api || {};

    // Sign into the system
    api.login = function (username, password) {

        // Create post data
        var postData = {
            client_id: 'KryptPadWeb',
            grant_type: 'password',
            username: username,
            password: password
        };

        // Send to token endpoint
        return $.ajax({
            type: 'POST',
            url: '/token',
            data: postData
        });
    };

    // Register an account
    api.register = function (email, password, confirmPassword) {
        var postData = {
            email: email,
            password: password,
            confirmPassword: confirmPassword
        };

        return $.ajax({
            type: 'POST',
            url: '/api/account/register',
            data: postData
        });
    };

    // Issues an email to the user with a link to reset password
    api.forgotPassword = function (email) {

        var postData = {
            email: email
        };

        return $.ajax({
            type: 'POST',
            url: '/api/account/forgot-password',
            data: postData
        })
    };

    // Reauthenticate
    api.reauthenticate = function (token) {

        // Create post data
        var postData = {
            client_id: 'KryptPadWeb',
            grant_type: 'refresh_token',
            refresh_token: token.refresh_token
        };

        // Send to token endpoint
        return $.ajax({
            type: 'POST',
            url: '/token',
            data: postData
        });
    };

    // Account related apis
    api.getAccountDetails = function () {
        return authorizedAjax({
            type: 'GET',
            url: '/api/account/details'
        });

    };

    // Sends the user a link so that he/she can confirm his/her email address
    api.sendEmailConfirmationLink = function () {
        return authorizedAjax({
            type: 'POST',
            url: '/api/account/send-email-confirmation-link'
        });

    };

    // Changes the user account password
    api.changePassword = function (currentPassword, newPassword, confirmPassword) {
        // Send all the data we need to the api to reset the password
        var postData = {
            currentPassword: currentPassword,
            newPassword: newPassword,
            confirmPassword: confirmPassword
        };

        return authorizedAjax({
            type: 'POST',
            url: '/api/account/change-password',
            data: postData
        })
    };

    // Reset password
    api.resetPassword = function (userId, code, password, confirmPassword) {

        var postData = {
            userId: userId,
            code: code,
            password: password,
            confirmPassword: confirmPassword
        };

        return authorizedAjax({
            type: 'POST',
            url: '/api/account/reset-password',
            data: postData
        });
    };

    // Gets a list of profiles for the user
    api.getProfiles = function () {
        return authorizedAjax({
            type: 'GET',
            url: '/api/profiles'
        });

    };

    /*
     * Gets a list of items
     */
    api.getItems = function (profileId, passphrase) {
        return authorizedAjax({
            type: 'GET',
            url: '/api/profiles/'+ profileId + '/categories/with-items',
            headers: { Passphrase: passphrase }
        });

    };

    /*
     * Creates an authorization header for web api calls. If the token is about to expire, the refresh token is sent and a new access token is retrieved.
     */
    function authorizedAjax(ajaxOptions) {
        // Get the token object
        var token = app.getToken();
        if (!token) {
            // Initiate logout
            app.logout();
        }

        // get the expiration date from our token
        var expires = moment(token['.expires']);
        
        // Is our access token about to expire?
        if (expires.isBefore(moment())) {
            console.warn('Access token has expired. Reauthenticating with refresh token.')
            // Get the new access token
            token = api.reauthenticate(token);
        }

        return $.when(token).then(function (data) {
            // Cache the access token in session storage.
            app.setToken(data);

            // Check for headers. Create if not there
            if (!ajaxOptions.headers) {
                ajaxOptions.headers = {};
            }
           
            // Check to see if we have an access token
            if (data) {
                ajaxOptions.headers.Authorization = 'Bearer ' + data.access_token;
            }

            // Execute the ajax request based on our ajax options
            return $.ajax(ajaxOptions);
        }).fail(function (error) {
            // Does this mean we failed to authenticate?
            if (error.responseJSON && error.responseJSON.error === 'invalid_grant') {
                // Initiate logout
                app.logout();
            }
        });

    };
})(window);
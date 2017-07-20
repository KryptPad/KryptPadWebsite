﻿(function (global) {

    // Create Api object
    var api = global.api = global.api || {};

    // Create a route table for the api
    api.routes = {
        tokenSignin: "/token",
        signin: null,
        createProfile: '/api/profiles'
    };

    // Sign into the system
    api.tokenSignin = function (email, password) {

        // Create post data
        var postData = {
            client_id: 'KryptPadWeb',
            grant_type: 'password',
            username: email,
            password: password
        };

        // Send to token endpoint
        return $.ajax({
            type: 'POST',
            url: api.routes.tokenSignin,
            data: postData
        });
    };

    // Sign into the system
    api.signin = function (email, password, requestUrl, antiForgeryToken) {
        
        // Create post data
        var postData = {
            email: email,
            password: password,
            __RequestVerificationToken: antiForgeryToken
        };

        // Send to token endpoint
        return $.ajax({
            type: 'POST',
            url: api.routes.signin,
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

    /*
     * Gets a list of profiles for the user
     */
    api.getProfiles = function () {
        return authorizedAjax({
            type: 'GET',
            url: '/api/profiles'
        });

    };

    /*
     * Loads a profile using the passphrase
     */
    api.loadProfile = function (id, passphrase) {
        return authorizedAjax({
            type: 'GET',
            url: '/api/profiles/' + id
        }, passphrase);

    };

    /*
     * Gets a list of items
     */
    api.getItems = function (profileId) {
        // Get the passphrase
        var passphrase = app.getPassphrase();
        // Make api call with passphrase
        return authorizedAjax({
            type: 'GET',
            url: '/api/profiles/'+ profileId + '/categories/with-items'
        }, passphrase);

    };

    /*
     * Save profile
     */
    api.saveProfile = function (profileId, data) {
        // Make api call to POST if new, and PUT if not new
        if (!profileId) {
            // This is a new profile
            return authorizedAjax({
                type: 'POST',
                url: api.routes.createProfile,
                data: data
            });
        } else {
            // This is an existing profile, update it
        }
       
    };

    /*
     * Creates an authorization header for web api calls. If the token is about to expire, the refresh token is sent and a new access token is retrieved.
     */
    function authorizedAjax(ajaxOptions, passphrase) {
        // Get the token object
        var token = app.getToken();
        if (token) {
            // get the expiration date from our token
            var expires = moment(token['.expires']);

            // Is our access token about to expire?
            if (expires.isBefore(moment())) {
                console.warn('Access token has expired. Reauthenticating with refresh token.');
                // Get the new access token
                token = api.reauthenticate(token);
            }
        }

        // When token is ready and available
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

            // Check to see if we have a passphrase
            if (passphrase) {
                ajaxOptions.headers.Passphrase = passphrase;
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
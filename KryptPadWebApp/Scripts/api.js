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

    }

    // Gets a list of profiles for the user
    api.getProfiles = function () {
        return authorizedAjax({
            type: 'GET',
            url: '/api/profiles'
        });

    }

    // Creates an authorization header for web api calls. If the token is about to expire,
    // the refresh token is sent and a new access token is retrieved.
    function authorizedAjax(ajaxOptions) {
        // Get the token object
        var token = app.getToken();
        
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
            // Create an object for our header
            var headers = {};

            // Check to see if we have an access token
            if (data) {
                headers.Authorization = 'Bearer ' + data.access_token;
            }

            // Set the header with the new access token
            ajaxOptions.headers = headers;

            // Execute the ajax request based on our ajax options
            return $.ajax(ajaxOptions);
        });

    };
})(window);
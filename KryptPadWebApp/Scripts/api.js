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
    }


    // Account related apis
    api.getAccountDetails = function () {
        // Return promise
        return $.ajax({
            type: 'GET',
            url: '/api/account/details',
            headers: authorizeHeader()
        });
    }


    // Creates an authorization header for web api calls
    function authorizeHeader() {
        // Create an object for our header
        var headers = {};
        // Get the token object
        var token = app.getToken();
        // Check to see if we have an access token
        if (token) {
            headers.Authorization = 'Bearer ' + token.access_token;
        }

        return headers;
    };
})(window);
(function (global) {
    var app = global.app = global.app || {};

    // Define some app level variables

    // This represents the key for our token response
    app.tokenKey = 'token';
    // Define some functions for our app

    // Gets the token object from storage
    app.getToken = function () {
        // Return the token response
        var tokenData = sessionStorage.getItem(app.tokenKey);
        // Deserialize the data to a token response
        return tokenData ? JSON.parse(tokenData) : null;
    };

    // Sets the token object in storage
    app.setToken = function (token) {
        // Serialize the token response into a string for session storage
        var tokenData = JSON.stringify(token);
        // Store the token result in session storage
        sessionStorage.setItem(app.tokenKey, tokenData);
    }

    // Helper method to get user name from the token
    app.getUserName = function () {
        // Get the token data and return the user name
        var token = app.getToken();
        // If we have some token data, return the user name
        if (token) {
            return token.userName;
        }
        else {
            return null;
        }
    }

    // Clears the token from storage
    app.logout = function () {
        // Clear the session token
        sessionStorage.removeItem(app.tokenKey);
        // Go to sign in page
        global.location = "/app";
    }

    // Returns true if the user is authenticated
    app.isAuthenticated = function () {
        // Check for the token in the session storage. If it is there, then
        // the user is authenticated.
        return app.getToken() ? true : false;
    };

    // Creates an authorization header for web api calls
    app.authorizeHeader = function () {
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

    app.processError = function (error, fail) {
        var message = '';
        
        // Get the error message
        if (error.responseJSON && error.responseJSON.error_description) {
            // Get the error message from OAuth
            message = error.responseJSON.error_description;

        } else if (error.responseJSON && error.responseJSON.Message) {
            // Get the error message from .net
            message = error.responseJSON.Message;

            if (error.responseJSON.ModelState) {
                var state = error.responseJSON.ModelState;
                var errors = [];
                // Get all the errors
                for (var property in state) {
                    
                    if (state.hasOwnProperty(property)) {
                        // Add errors to the list
                        errors.push(state[property].join("<br />"));
                        
                    }
                }

                message += '<br />' + errors.join('<br />');
            }

        } else {
            message = "An unknown error has occurred.";
        }

        // Determine what to do with an error response
        if (error.status == 401) {
            // The web call was unauthorized
            message = "You are not authorized to perform this action.";
            // Sign out
            app.logout();
        }

        // Call the fail callback
        if (typeof fail === 'function') {
            fail(message);
        }
    }

})(window);
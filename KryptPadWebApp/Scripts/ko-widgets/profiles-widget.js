ko.components.register('profiles-widget', {
    viewModel: function (params) {
        var self = this;

        self.profiles = ko.observableArray([]);
        self.isBusy = ko.observable(false);
        self.errorMessage = ko.observable();

        self.getProfiles = function () {

            // Set busy state
            self.isBusy(true);

            // Authorize the request
            $.ajax({
                type: 'GET',
                url: '/api/profiles',
                headers: app.authorizeHeader()
            }).done(function (data) {
                // Bind the profiles to the list
                $.each(data.Profiles, function () {
                    self.profiles.push(this);
                });

            }).fail(function (error) {
                app.processError(error, function (message) {
                    self.errorMessage(message);
                });
            }).complete(function () {
                // Set busy state
                self.isBusy(false);
            });
        }

        // Load the profiles
        self.getProfiles();
    },
    template: { fromUrl: 'profiles-widget.html' }
});

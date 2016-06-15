(function (global) {

    // Get main app container
    var node = document.getElementById('signin');

    // App view model
    function AppViewModel() {
        var self = this;

        
    }

    // Create model
    var model = new AppViewModel();

    // Apply bindings
    ko.applyBindings(model, node);

})(window);
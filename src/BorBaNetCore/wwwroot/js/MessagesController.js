(function () {
    'use strict';

    var controllerId = 'MessagesController';

    angular.module('MessagesApp').controller(controllerId,
        ['$scope', 'messageFactory', MessagesController]);

    function MessagesController($scope, messageFactory) {
        $scope.messages = [];

        messageFactory.getMessages().success(function (data) {
            $scope.messages = data;
        }).error(function (error) {
            // log errors
        });
    }

})();

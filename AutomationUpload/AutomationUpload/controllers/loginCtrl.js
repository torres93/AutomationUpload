
var app = angular.module("AutomationUpload");

app.controller("loginCtrl", function ($scope, $log, $location, authUsers, $mdDialog) {
    $scope.login = function (user) {
        if (user != undefined) {
            authUsers.login(user);
        }
        else {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Aviso')
                    .content('Ingresa tus datos')
                    .ok('Aceptar')
                )
        }

    }


})

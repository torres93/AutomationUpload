
var app = angular.module("AutomationUpload");


app.controller("adminCtrl", function ($scope, $http, sesionesControl, authUsers, $location, $mdDialog) {
    $scope.addUser = "normal";
    $http.post("wsApp.asmx/getUsuarios").success(function ($response) {
        $scope.usuarios = $response;
    });
    $http.post("wsApp.asmx/getFuentes").success(function ($response) {
        $scope.fuentes = $response;
    });
    $scope.selected = [];
    $scope.editarUsuario = function (usr) {
        $scope.userEdit = usr;
        console.log(usr.fuentes);
        $scope.selected = usr.fuentes;

    }


    $scope.toggle = function (fuentes, list) {
        var aux = "";
        var cont = 0;
        if (list.length > 0) {
            for (var i = 0; i < list.length; i++) {
                if (list[i].nombre == fuentes.nombre) {
                    console.log(list[i].nombre + " " + fuentes.nombre);
                    aux = "existe";
                    cont = i;

                }
            }
            console.log(aux);
            console.log(cont);
            if (aux == "existe") {
                console.log(cont);
                list.splice(cont, 1);
            }
            else {
                list.push(fuentes);
            }
        } else {
            list.push(fuentes);
        }


    };
    $scope.exist = function (fuentes, list) {
        var str = JSON.stringify(list);
        var f = JSON.stringify(fuentes);
        return str.indexOf(f) > -1;
    }

    $scope.editUser = function (userEdit) {
        console.log(userEdit);
        userEdit.fuentes = $scope.selected;

        $http.post("wsApp.asmx/updateUsuario", userEdit).success(function ($response) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Aviso')
                    .content('Cambios guardados')
                    .ok('Aceptar')
            )
            $scope.userEdit = "";
            $scope.selected = [];
        });

    }
    $scope.creaarUsuario = function (userN) {
        userN.fuentes = $scope.selected;
        $http.post("wsApp.asmx/createUsuario", userN).success(function ($response) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Aviso')
                    .content('Nuevo usuario creado')
                    .ok('Aceptar')

            )
            $scope.selected = [];
            $scope.userN = "";
            $scope.addUser = "normal";

            $http.post("wsApp.asmx/getUsuarios").success(function ($response) {
                $scope.usuarios = $response;
            });
        })
    }

})



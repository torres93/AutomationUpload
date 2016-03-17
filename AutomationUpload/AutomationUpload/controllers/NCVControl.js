var app = angular.module("AutomationUpload");
app.controller("NCVCtrl", function ($scope, $http, authUsers, sesionesControl, $mdDialog) {
    $scope.vistaValidada = true;
    $scope.validaVista = function (catalogoV1, catalogoV2) {
        var m = JSON.stringify({ vista: catalogoV1, tabla: catalogoV2 });
        $http.post("wsApp.asmx/validaCargaVista", m).success(function ($response) {

            if ($response.d == "exception") {
                $mdDialog.show(
                             $mdDialog.alert()
                             .clickOutsideToClose(true)
                             .title('Ocurrio Algun Error!')
                             .content(' Verifica que las 2 tablas tengan la misma estructura')
                             .ok('Aceptar')
                             )
            } else {
                $scope.validarV = JSON.parse($response.d);





                if ($scope.validarV[0].cont == '0' && $scope.validarV[0].existentrows == '0') {
                    $mdDialog.show(
                      $mdDialog.alert()
                      .clickOutsideToClose(true)
                      .title('Valicación completa')
                      .content('se valido que: 1) la vista no tiene duplicados 2) la vista no tiene valores ya cargados en la tabla de cifras')
                      .ok('Aceptar')
                      )
                    $scope.vistaValidada = false;

                    //$http.post("wsApp.asmx/validaCargaVista", m).success(function ($response) {
                    //    console.log($response.d);

                    //});



                } else if ($scope.validarV[0].cont != '0') {
                    $mdDialog.show(
                      $mdDialog.alert()
                      .clickOutsideToClose(true)
                      .title('La vista' + catalogoV1 + ' tiene ' + $scope.validarV[0].cont + ' lineas Repetidas')
                      .content()
                      .ok('Aceptar')
                      )
                } else if ($scope.validarV[0].existentrows != '0') {
                    $mdDialog.show(
                      $mdDialog.alert()
                      .clickOutsideToClose(true)
                      .title($scope.validarV[0].existentrows + ' lineas ya se encuentran registradas en ' + catalogoV2)
                      .content()
                      .ok('Aceptar')
                      )
                } else {
                    $mdDialog.show(
                      $mdDialog.alert()
                      .clickOutsideToClose(true)
                      .title('Existen lineas Repetidos en ' + catalogoV1 + ' y ' + catalogoV2)
                      .content()
                      .ok('Aceptar')
                      )
                }
            }

        });
    }
    $scope.configVista = function (encuesta) {

        for (var i = 0; i < $scope.encuestas.length; i++) {
            if ($scope.encuestas[i].nombre == encuesta) {
                modelo = $scope.encuestas[i].id_modelo;
            }
        };
        modeloJson = JSON.stringify({ model: modelo });
        $http.post("wsApp.asmx/getCampoCatalogo", modeloJson).success(function ($response) {
            $scope.campos = JSON.parse($response.d);
        });
    }
    $scope.replicaVista = function () {
        $http.post("wsApp.asmx/replicaVista", modeloJson).success(function ($response) {
            if ($response.d == "") {

                $mdDialog.show(
                             $mdDialog.alert()
                             .clickOutsideToClose(true)
                             .title('Ocurrio Algun Error!')
                             .content(' La replica se realizó satisfactoriamente')
                             .ok('Aceptar')
                             )
            } else {
                $mdDialog.show(
                             $mdDialog.alert()
                             .clickOutsideToClose(true)
                             .title('Ocurrio Algun Error!')
                             .content(' Ocurrio algun error al realizar la replica de la informacion ')
                             .ok('Aceptar')
                             )
                    $scope.vistaValidada = true;

            }
        });
    }

})
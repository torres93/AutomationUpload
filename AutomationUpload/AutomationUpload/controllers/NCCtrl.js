var app = angular.module("AutomationUpload");


app.controller("userCtrl", function ($scope, $http, authUsers, sesionesControl, $mdDialog) {

    $http.post("wsApp.asmx/getModelos").success(function ($response) {
        //console.log($response);
        $scope.modelos = $response;
    });

    $scope.configModeloVista = function (modelo) {
        var res;
        for (var i = 0; i < $scope.modelos.length; i++) {
            if ($scope.modelos[i].nombre == modelo) {
                res = $scope.modelos[i].id_modelo;
            }
        }
        $scope.id_modelo = res;
        var m = JSON.stringify({ modelo: res });
        $http.post("wsApp.asmx/getEncuestas", m).success(function ($response) {
            //console.log($response);
            $scope.encuestas = JSON.parse($response.d);
        })
        $http.post("wsApp.asmx/getCatalogosModelo", m).success(function ($response) {
            //console.log($response);
            $scope.catalogos = JSON.parse($response.d);
        });
    }

    $scope.selected = [];
    $scope.toggle = function (campos, list) {
        var idx = list.indexOf(campos.nombre);
        if (idx > -1) list.splice(idx, 1);
        else list.push(campos.nombre);
    };
    $scope.exist = function (campos, list) {
        return list.indexOf(campos.nombre) > -1;
    }
})

app.controller("NCCtrl", ["$scope", "$http", "$au_validator", "fileUpload", "$mdDialog", function ($scope, $http, $validator, fileUpload, $mdDialog) {
    $scope.pathFile = "";
    $scope.path = "";
    $scope.data;
    $scope.VdE = false;
    $scope.VdDI = false;
    $scope.tablaFull = false;
    $scope.fnValidate = function () {
        if ($scope.$parent.id_modelo != null && $scope.$parent.id_modelo != "") {
            var n = $scope.$parent.id_modelo
            if ($scope.data != null && $scope.data != "") {
                $validator.fnValidateLength(n, $scope.data).then(function (res) {
                    var d = JSON.parse(res.data.d);
                    var validation = false;
                    if (d.length == $scope.data[0].length) {

                        for (i = 0; i < $scope.data[0].length; i++) {
                            for (x = 0; x < d.length; x++) {
                                if (d[x].nombre == $scope.data[0][i]) {
                                    validation = true;
                                    break;
                                }
                                else {
                                    validation = false;
                                }
                            }
                            if (validation == false) {
                                break;
                            }
                        }

                        $scope.VdE = validation;
                        console.log($scope.VdE);

                    }
                    else {

                        $scope.VdE = false;
                        console.log($scope.VdE);
                    }


                    if ($scope.VdE == true) {
                        var json = JSON.stringify({
                            jsonobj: $scope.data
                        })
                        $http.post("wsApp.asmx/Duplicitywatcher", json).success(function ($response) {
                            console.log($response);
                            $scope.VdDI = $response.d;

                            if($scope.VdDI==true)
                            {
                                console.log($scope.VdE);
                                $mdDialog.show(
                               $mdDialog.alert()
                                  .clickOutsideToClose(true)
                                  .title('Aviso')
                                  .content('Se completo toda la validacion')
                                  .ok('Aceptar')
                              )
                            }
                            else {
                                console.log($scope.VdE);
                                $mdDialog.show(
                               $mdDialog.alert()
                                  .clickOutsideToClose(true)
                                  .title('Aviso')
                                  .content('No  paso la validacion de duplicidad interna')
                                  .ok('Aceptar')
                              )
                            }

                        }).error(function (x, y, z) {

                        })
                    }       
                    
                    else {
                        console.log($scope.VdE);
                        $mdDialog.show(
                       $mdDialog.alert()
                          .clickOutsideToClose(true)
                          .title('Aviso')
                          .content('No  paso la validacion de estructura')
                          .ok('Aceptar')
                      )
                    }
                });


            }


            else {
                $mdDialog.show(
                $mdDialog.alert()
                   .clickOutsideToClose(true)
                   .title('Aviso')
                   .content('No se ha seleccionado un archivo a validar')
                   .ok('Aceptar')
               )
            }
        }
        else {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Aviso')
                    .content('No se ha seleccionado un "Modelo"')
                    .ok('Aceptar')
                )
        }
    }


    $scope.fnReplicate = function () {
        if ($scope.VdE == true && $scope.VdDI == true) {

            var json = JSON.stringify({
                jsonobj: $scope.data
            })
            $http.post("wsApp.asmx/insertWorkTable", json).success(function ($response) {

                $mdDialog.show(
          $mdDialog.alert()
             .clickOutsideToClose(true)
             .title('Aviso')
             .content('Filas afectadas:' + $response.d)
             .ok('Aceptar'))

            }).error(function (x, y, z) {
                console.log(x);
                console.log(y);
            })
        }
        else {
            $mdDialog.show(
           $mdDialog.alert()
              .clickOutsideToClose(true)
              .title('Aviso')
              .content('No se pasaron con exito todas las validaciones')
              .ok('Aceptar'))
        }
    }
    $scope.fnBringTable = function () {

        var file = $scope.archivo;
        fileUpload.validatorFile(file).then(function (res) {
            $scope.data = res.data;
            if ($scope.data != "") {
                $scope.tablaFull = true;
            }
            else {
                $scope.tablaFull = false;
                $mdDialog.show(
                $mdDialog.alert()
                   .clickOutsideToClose(true)
                   .title('Aviso')
                   .content('No se ha seleccionado un archivo para mostrar o el archivo presento algun error')
                   .ok('Aceptar'))
            }
        });
    };
    $scope.NotePath = function () {

        $scope.$apply(function () {
            fullName = $('#fileSearch').val();
            shortName = fullName.match(/[^\/\\]+$/);
            $scope.pathShort = $('#filepath').value = shortName;
            $scope.path = fullName;
        })

    }
}])
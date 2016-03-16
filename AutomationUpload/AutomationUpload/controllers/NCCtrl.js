var app = angular.module("AutomationUpload");


app.controller("userCtrl", function ($scope, $http, authUsers, sesionesControl, $mdDialog) {

    $http.post("wsApp.asmx/getModelos").success(function ($response) {
        //console.log($response);
        $scope.modelos = $response;
    });

    $scope.validaVista = function (catalogoV1, catalogoV2)
    {
        var m = JSON.stringify({ vista: catalogoV1, tabla: catalogoV2 ,insTbl:''});
        $http.post("wsApp.asmx/validaCargaVista", m).success(function ($response) {
            
            if ($response.d== "nomms") {
                $mdDialog.show(
                             $mdDialog.alert()
                             .clickOutsideToClose(true)
                             .title('Campos de tablas diferentes')
                             .content()
                             .ok('Aceptar')
                             )
            } else {
                $scope.validarV = JSON.parse($response.d);

                if ($scope.validarV[0].cont == '0' && $scope.validarV[0].existentrows == '0') {
                            $mdDialog.show(
                              $mdDialog.alert()
                              .clickOutsideToClose(true)
                              .title('Valicación completa')
                              .content()
                              .ok('Aceptar')
                              )
                            var m = JSON.stringify({ insTbl: catalogoV1, vista: '', tabla: '' });
                            $http.post("wsApp.asmx/validaCargaVista", m).success(function ($response) {
                            });

                } else if ($scope.validarV[0].cont != '0') {
                            $mdDialog.show(
                              $mdDialog.alert()
                              .clickOutsideToClose(true)
                              .title('Existen '+$scope.validarV[0].cont+' lineas Repetidos en ' +catalogoV1 )
                              .content()
                              .ok('Aceptar')
                              )
                } else if ($scope.validarV[0].existentrows != '0') {
                    $mdDialog.show(
                      $mdDialog.alert()
                      .clickOutsideToClose(true)
                      .title('Existen ' + $scope.validarV[0].existentrows + ' lineas Repetidos en ' + catalogoV2)
                      .content()
                      .ok('Aceptar')
                      )
                } else {
                    $mdDialog.show(
                      $mdDialog.alert()
                      .clickOutsideToClose(true)
                      .title('Existen lineas Repetidos en ' + catalogoV1 +' y '+catalogoV2)
                      .content()
                      .ok('Aceptar')
                      )
                }
            }
       
        });
    }
   
    $scope.configModeloVista = function (modelo)
    {
        var res;
        for (var i = 0; i < $scope.modelos.length; i++) {
            if ($scope.modelos[i].nombre == modelo) {
                 res = $scope.modelos[i].id_modelo;
            }
        }
        $scope.id_modelo = res;
        var m = JSON.stringify({ modelo: res });
        $http.post("wsApp.asmx/getEncuestas",m).success(function ($response) {
            //console.log($response);
            $scope.encuestas = JSON.parse($response.d);
        })
        $http.post("wsApp.asmx/getCatalogosModelo",m).success(function ($response) {
            //console.log($response);
            $scope.catalogos = JSON.parse($response.d);
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

app.controller("NCCtrl", ["$scope", "$http", "$au_validator", "fileUpload", function ($scope, $http, $validator, fileUpload) {
    $scope.pathFile = "";
    $scope.path = "";
    $scope.data;
    $scope.fnValidate = function()
    {
        if($scope.data!=null)
        {
            var n = $scope.$parent.id_modelo
            $validator.fnValidateLength(n);
        }
        else {
            alert("No mi chavo primero anexe un archivo...")
        }
    }
    $scope.fnBringTable = function () {

        var file = $scope.archivo;
        fileUpload.validatorFile(file).then(function (res) {
            $scope.data = res.data;
            document.getElementById("tableContainer").removeAttribute("hidden");
        });
    };
    $scope.NotePath = function()
    {
     
        $scope.$apply(function()
        {
            fullName = $('#fileSearch').val();
            shortName = fullName.match(/[^\/\\]+$/);
            $scope.pathShort= $('#filepath').value = shortName;
            $scope.path = fullName;
        })

    }
}])

var app = angular.module("AutomationUpload");

app.controller("adminModelCtrl", function ($scope, $http, sesionesControl, authUsers, $location) {
    var modelo = "";
    $scope.nuevosCatalogo = []
    $scope.nuevosCatalogo.nombreCampo = ""
    $scope.nuevosCatalogo.nombreCatalogo = ""
    $scope.nuevosCatalogo.nombreFuente = ""
    $scope.nuevosCatalogo.idFuente = 0
    var catalogosSeleccionados;

    $scope.$watch('nuevosCatalogo.idFuente', function (newValue, oldValue) {
        if (newValue == null) {
            $scope.nuevosCatalogo.idFuente = 0;
        }
    });
    $http.post("wsApp.asmx/getModelos").success(function ($response) {
        $scope.modelos = $response;
    });
    $http.post("wsApp.asmx/getCatalogos").success(function ($response) {
        for (i = 0; i < $response.length; i++) {
            $response[i].selected = false;
        }
        $scope.catalogos = $response;
    });

    $scope.cambiaModelo = function (m) {
        modelo = m.id_modelo

        campoJson = JSON.stringify({ modelo: modelo });

        $http.post("wsApp.asmx/getFuentesModelo", campoJson).success(function ($response) {
            $scope.fuentes = JSON.parse($response.d);
        });
        $http.post("wsApp.asmx/getCatalogosModelo", campoJson).success(function ($response) {
            var cat = JSON.parse($response.d);
            catalogosSeleccionados = new JSON.constructor();
            for (var i = 0; i < $scope.catalogos.length; i++) {
                var select = false;
                for (var j = 0; j < cat.length; j++) {
                    if (cat[j].id_catalogo == $scope.catalogos[i].id_catalogo) {
                        select = true;
                        break;
                    }
                }
                catalogosSeleccionados[i] = select;
                $scope.catalogos[i].selected = select;

            }
            console.log(catalogosSeleccionados);
        });

    }

    $scope.actualizaAddCatalogo = function () {
        if ($scope.nuevosCatalogo.nombreCampo != "") {

            campoJson = JSON.stringify({ nombre: $scope.nuevosCatalogo.nombreCampo, catalogo: 1, modelo: modelo, fuente: $scope.nuevosCatalogo.idFuente });
            $http.post("wsApp.asmx/insCampo", campoJson).success(function ($response) {

            });
        }
        if ($scope.nuevosCatalogo.nombreCatalogo != "") {
            campoJson = JSON.stringify({ nombre: $scope.nuevosCatalogo.nombreCatalogo, catalogo: 2, modelo: modelo, fuente: $scope.nuevosCatalogo.idFuente });
            $http.post("wsApp.asmx/insCampo", campoJson).success(function ($response) {

                $http.post("wsApp.asmx/getCatalogos").success(function ($response) {
                    $scope.catalogos = $response;
                });
            });
        }
        if ($scope.nuevosCatalogo.nombreFuente != "" && $scope.nuevosCatalogo.idFuente > 0 && !isNaN(modelo)) {
            campoJson = JSON.stringify({ nombre: $scope.nuevosCatalogo.nombreFuente, catalogo: 3, modelo: modelo, fuente: $scope.nuevosCatalogo.idFuente });
            $http.post("wsApp.asmx/insCampo", campoJson).success(function ($response) {

            });
        }
    }

    $scope.actualizaTI_ER = function () {

        console.log(catalogosSeleccionados);
        console.log($scope.catalogos);
        //for (var i = 0; i < $scope.catalogos.length; i++) {
        //    if (($scope.catalogos[i].selected && catalogosSeleccionados[i]) || (!$scope.catalogos[i].selected && !catalogosSeleccionados[i])) {
        //        console.log($scope.catalogos[i].nombre + " no tiene cambios");
        //    }
        //    if (($scope.catalogos[i].selected && !catalogosSeleccionados[i])) {
        //        console.log($scope.catalogos[i].nombre + " se puso");
        //    }
        //    if ((!$scope.catalogos[i].selected && catalogosSeleccionados[i])) {
        //        console.log($scope.catalogos[i].nombre + " se quito");
        //    }
        //}

        var idcatalogos = [];
        for (var i = 0; i < $scope.catalogos.length; i++)
        {
            if ($scope.catalogos[i].selected)
            {
                idcatalogos.push($scope.catalogos[i].id_catalogo);
            }
           
        }

        var jsonupdate = {
            idmodelo: modelo,
            idcatalogos:idcatalogos
        }
        jsonupdate = JSON.stringify(jsonupdate);
        $http.post("wsApp.asmx/updateModeloCatalogo",jsonupdate).success(function ($response) {

            alert($response.d);

        }).error(function(x,y)
        {
            console.log(x);
            console.log(y);
        })


    }



})



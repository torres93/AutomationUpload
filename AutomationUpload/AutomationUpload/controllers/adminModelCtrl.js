var app = angular.module("AutomationUpload");

app.controller("adminModelCtrl", function ($scope, $http, sesionesControl, authUsers, $location) {
    var modelo = "";
    var catalogo = "";
    $scope.nuevosCatalogo = []
    $scope.nuevosCatalogo.nombreCampo = ""
    $scope.nuevosCatalogo.nombreCatalogo = ""
    $scope.nuevosCatalogo.nombreFuente = ""
    $scope.nuevosCatalogo.idFuente = 0
    var catalogosSeleccionados;
    var camposSeleccionados;
    // campoEmpt es un campo con valores vacios ques solo se utiliza para limpiar los input de edicion 
    campoEmpt = new JSON.constructor();
    campoEmpt.nombre = '';
    campoEmpt.descripcion = '';
    campoEmpt.llave = '';
    campoEmpt.nulos = '';
    campoEmpt.id_tipo = '';
    campoEmpt.tipo = '';
    $scope.campoEdit = campoEmpt;
    //////////////////////////////////////////////////////////////////////////////////////////////////
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
    $http.post("wsApp.asmx/getCampos").success(function ($response) {
        for (i = 0; i < $response.length; i++) {
            $response[i].selected = false;
            $response[i].descripcion = '';
            $response[i].llave = false;
            $response[i].nulos = false;
            $response[i].tipo = '';
            $response[i].id_tipo = '';
        }
        $scope.campos = $response;
    });
    $http.post("wsApp.asmx/getTipoDatos").success(function ($response) {
        $scope.tiposDato = $response;
    });

    $scope.cambiaModelo = function (m) {
        modelo = m.id_modelo

        campoJson = JSON.stringify({ modelo: modelo });

        $http.post("wsApp.asmx/getFuentesModelo", campoJson).success(function ($response) {
            $scope.fuentes = JSON.parse($response.d);
        });
        $http.post("wsApp.asmx/getCatalogosDelModelo", campoJson).success(function ($response) {
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

            if ($response != "") {
                $scope.catalogModelo = JSON.parse($response.d);

            }
        });

    }

    $scope.cambiaCatalogoModelo = function (cm) {
        catalogo = cm.id_catalogo
        campoJson = JSON.stringify({ modelo: modelo, catalogo: catalogo });

        $http.post("wsApp.asmx/getCamposCatalogoModelo", campoJson).success(function ($response) {
            if ($response.d != "") {
                camposCatalogo = JSON.parse($response.d);
                camposSeleccionados = new JSON.constructor();
                for (var i = 0; i < $scope.campos.length; i++) {
                    var selected = false;
                    for (var j = 0; j < camposCatalogo.length; j++) {
                        if ($scope.campos[i].id_campo == camposCatalogo[j].id_campo) {
                            $scope.campos[i] = camposCatalogo[j];
                            selected = true;
                            break;
                        }
                    }
                    camposSeleccionados[i] = selected;
                    $scope.campos[i].selected = selected;




                }
            } else {

                for (var i = 0; i < $scope.campos.length; i++) {
                    camposSeleccionados[i] = false;
                    $scope.campos[i].selected = false;
                }
            }

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
        for (var i = 0; i < $scope.catalogos.length; i++) {
            if ($scope.catalogos[i].selected) {
                idcatalogos.push($scope.catalogos[i].id_catalogo);
            }

        }

        var jsonupdate = {
            idmodelo: modelo,
            idcatalogos: idcatalogos
        }
        jsonupdate = JSON.stringify(jsonupdate);
        $http.post("wsApp.asmx/updateModeloCatalogo", jsonupdate).success(function ($response) {



        }).error(function (x, y) {
            console.log(x);
            console.log(y);
        })


        ////////////////////////////////////////////////////////////////////////////////////////




    }


    $scope.actualizaTR_TABLA = function (c) {

        if (c.descripcion != '' && c.id_tipo != '') {
            if (c.selected) {
                //////////////////////////update
                campoJson = JSON.stringify({ catalogo: catalogo, modelo: modelo, campo: c.id_campo, descripcion: c.descripcion, llave: c.llave, nulo: c.nulos, tipo: c.id_tipo });

                $http.post("wsApp.asmx/updateCampoTabla", campoJson).success(function ($response) {

                    alert("Se actualizó el campo satisfactoriamente")
                });
            }
            else {
                /////////////////////////insert
                c.selected = true;

                campoJson = JSON.stringify({ catalogo: catalogo, modelo: modelo, campo: c.id_campo, descripcion: c.descripcion, llave: c.llave, nulo: c.nulos, tipo: c.id_tipo });

                $http.post("wsApp.asmx/insCampoTabla", campoJson).success(function ($response) {

                    alert("Se insertó el campo satisfactoriamente")
                });
            }
        } else {
            alert("No se llenaron correctamente los datos")
        }


        ////////////////////////////////////////////////////////////////////////////////////////




    }
    $scope.addCampo = function (c) {
        for (var i = 0; i < $scope.campos.length; i++) {
            if ($scope.campos[i].id_campo == c.id_campo) {
                $scope.campos[i].selected = true;
                break;
            }

        }
    }
    $scope.editarCampo = function (c, selected) {
        console.log(catalogo)
        if (catalogo != "") {
            if (selected) {

                $scope.campoEdit = c;

            } else {
                ///////////aqui se debe de eliminar
                c.selected = selected;

                campoJson = JSON.stringify({ catalogo: catalogo, modelo: modelo, campo: c.id_campo });

                $http.post("wsApp.asmx/delCampoTabla", campoJson).success(function ($response) {

                    alert("Se eliminó el campo satisfactoriamente")
                });
            }
        } else {
            alert("No has seleccionado un catalogo y/o un modelo")
        }
    }
})



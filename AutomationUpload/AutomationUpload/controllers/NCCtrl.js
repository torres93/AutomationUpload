var app = angular.module("AutomationUpload");


app.controller("userCtrl", function ($scope, $http, authUsers, sesionesControl) {

    $http.post("wsApp.asmx/getModelos").success(function ($response) {
        console.log($response);
        $scope.modelos = $response;
    });
   

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
            console.log($response);
            $scope.encuestas = JSON.parse($response.d);
        })
    }

    $scope.configVista = function (encuesta) {

        for (var i = 0; i < $scope.encuestas.length; i++) {
            if ($scope.encuestas[i].nombre == encuesta) {
                modelo = $scope.encuestas[i].id_modelo;
            }
        };
        modeloJson = JSON.stringify({ model: modelo });
        $http.post("wsApp.asmx/getCampos", modeloJson).success(function ($response) {
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

app.controller("NCCtrl", ["$scope","$http","$au_validator", function ($scope,$http,$validator) {
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

        var data = JSON.stringify({

            file: $scope.path
        });
        $http.post("wsApp.asmx/getTable", data).success(function ($response) {
            console.log($response.d);
            $scope.data = JSON.parse($response.d);
            document.getElementById("tableContainer").removeAttribute("hidden");
            console.log($scope.data);
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
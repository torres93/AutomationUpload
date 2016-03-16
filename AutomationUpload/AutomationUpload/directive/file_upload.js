
var app = angular.module("AutomationUpload");
app.directive('fileModel', ['$parse', function ($parse) {
    return {
        restrict: 'A',
        link: function (scope, iElement, iAttrs) {

            iElement.on("change", function (e) {
                $parse(iAttrs.fileModel).assign(scope, iElement[0].files[0]);
            })

        }
    }
}]);


app.service('fileUpload', ['$http', '$q', '$mdDialog', function ($http, $q, $scope, $mdDialog) {
    this.validatorFile = function (file) {
        var deferred = $q.defer();
        var formData = new FormData();
        formData.append('file', file);
        
        return $http.post("wsApp.asmx/getTable", formData, {
            headers: {
                "Content-type": undefined
            },
            transformRequest: formData
        })
        .success(function (res) {
            deferred.resolve(res);
        })
        .error(function (msg, code) {

            deferred.reject(msg);
        })
        return deferred.promise;
    }
}]);
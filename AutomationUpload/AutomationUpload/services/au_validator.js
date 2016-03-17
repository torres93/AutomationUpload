

app.service("$au_validator", function ($http) {

    this.fnValidateLength = function (modelo,data) {
        alert("");
        var m = { id_modelo: modelo }
        $http.post("wsApp.asmx/getWorkTable", m).success(function ($response) {
            console.log($response.d);
            var d = JSON.parse($response.d);
            var validation = false;
            if (d.length == data[0].length)
            {
                
                for (i = 0; i < data[0].length; i++) {
                    for (x = 0; x < d.length; x++) {
                        if(d[x].nombre ==data [0][i] )
                        {
                            validation = true;
                            break;
                        }
                        else {
                            validation = false;
                        }
                    }
                    if(validation==false)
                    {
                        break;
                    }
                }
                return validation;
            }
            else {
                alert("la estructura de la tabla es diferente");
                return false;
            }

        });
    }

    this.InsertValues = function()
    {
        $http.post("wsApp.asmx/putWorkTable", m).success(function ($response) {
            console.log($response.d);
        });
    }
});
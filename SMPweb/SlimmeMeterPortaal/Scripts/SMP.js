$(document).ready(function () { SetLevels() });

function SetLevels() {
    //alert("yes");
    var syuse = document.getElementsByClassName("sy-use");
    //alert(syuse.length);

    if (syuse.length == 0) {
        return;
    }
    var sylvl = document.getElementsByClassName("sy-level");

    for (let i = 0; i < syuse.length; i++) {
        //alert(sylvl[i].value);
        var T = sylvl[i].value;
        // if (T != '0') { alert(T); }
        switch (T) {
            case '3':
                //alert("red")
                syuse[i].style.backgroundColor = "red";
                break;
            case '2':
                //alert("orange");
                syuse[i].style.backgroundColor = "orange";
                break;
            case '1':
                //alert("yellow");
                syuse[i].style.backgroundColor = "yellow";
                break;
            case '0':
                //alert("skyblue");
                syuse[i].style.backgroundColor = "skyblue";
                break;
            case '-1':
                //alert("greenyellow");
                syuse[i].style.backgroundColor = "greenyellow";
                break;
            case '-2':
                //alert("limegreen");
                syuse[i].style.backgroundColor = "limegreen";
                break;
            case '-3':
                //alert("darkgreen");
                syuse[i].style.backgroundColor = "darkgreen";
                break;
            default:
                alert("None???");
                break;
        }
    }
    alert("End");
    return;
}
    
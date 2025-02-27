$(document).ready(function () { SetLevels(); Navigate(); });

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
    //alert("End");
    return;
}

function Includeit(from) {
    //alert($(from).is(":checked"));
    var what = $(from).parent().next().text();
    //alert("===" + what + "===");
    if (what.includes("gas")) {
        //alert("gas");
        //alert("Before=" + document.getElementById('IncludeGas').value);
        if (from.checked) {
            document.getElementById('IncludeGas').value = "Y";
        }
        else {
            document.getElementById('IncludeGas').value = "N";
        }
        //alert("After=" + document.getElementById('IncludeGas').value);
    }
    else {
        //alert("stroom");
        //alert("Before=" + document.getElementById('IncludeStroom').value);
        if (from.checked) {
            document.getElementById('IncludeStroom').value = "Y";
        }
        else {
            document.getElementById('IncludeStroom').value = "N";
        }
        //alert("After=" + document.getElementById('IncludeStroom').value);
    }
    return;

}

function Navigate() {
    //alert("Navigate"); 
    var gas = document.getElementById("gas");
    var gaslink = document.getElementById("smp-gas");
    //alert(gas);
    if (gaslink != null) {        
        if (gas == null) {
            gaslink.remove();
        //    document.getElementById("smp-gas").style.visibility = "hidden";
        }
        //else {
        //    document.getElementById("smp-gas").style.visibility = "visible";
        //}
    }
    var stroom = document.getElementById("stroom");
    var stroomlink = document.getElementById("smp-stroom");
    //alert(stroom);
    if (stroomlink != null) {
        if (stroom == null) {
            stroomlink.remove();
        //    document.getElementById("smp-stroom").style.visibility = "hidden";
        }
        //else {
        //    document.getElementById("smp-stroom").style.visibility = "visible";
        //}
    }
}
    
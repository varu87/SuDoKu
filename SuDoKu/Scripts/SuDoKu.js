var cellInFocus, cellArray, startTime, endTime;

$(document).ready(function () {
    cellInFocus = '';
    cellArray = [];
    startTime = $.now();
    BindEvents();
});

var pge = Sys.WebForms.PageRequestManager.getInstance();
pge.add_endRequest(BindEvents);

function BindEvents() {
    GetHolesCount.count = 0;
    starTime = $.now();

    var timeoutId = 0;
    $('#btnUndo').on('mousedown', function () {
        timeoutId = setTimeout(UndoTillFlag, 1500);
    }).on('mouseup mouseleave', function () {
        clearTimeout(timeoutId);
    });

    $('#btnUndo').on('click', function () {
        if (cellArray !== 'undefined' && cellArray.length > 0) {
            var cell = cellArray.splice(cellArray.length - 1, 1) + '';
            $(cell).val('');
            if ($(cell).hasClass('restorePoint'))
                $(cell).removeClass('restorePoint');
        }
    });

    $('#btnReset').on('click', function () {
        if (cellArray !== 'undefined' && cellArray.length > 0) {
            var arrayLength = cellArray.length;
            var cell;
            for (var i = 0; i < arrayLength; i++) {
                cell = cellArray.splice(cellArray.length - 1, 1) + '';
                $(cell).val('');

                if ($(cell).hasClass('restorePoint')) {
                    $(cell).removeClass('restorePoint');
                }
            }
        }
    });

    $('#btnFlag').on('click', function () {
        if (typeof cellInFocus === "undefined")
            return;

        var cell = '#' + cellInFocus;
        if ($(cell).hasClass('restorePoint')) {
            $(cell).removeClass('restorePoint');
        }
        else {
            $(cell).addClass('restorePoint');
        }
    });
}

function ValidateCell(id, holesCount, event) {
    var cell = '#' + id;
    var data = $(cell).val();
    var key = event.which;

    if (key == 9 || key == 16 || key == 17 || key == 18 || key == 19 || key == 20 || key == 27 ||
    key == 33 || key == 34 || key == 35 || key == 36 || key == 37 || key == 38 || key == 39 || key == 40 ||
    key == 45 || key == 46 || key == 144 || key == 145)
        return;

    if (key == 8) {
        if (cellArray !== 'undefined' && cellArray.length > 0) {
            cellArray.splice(cellArray.indexOf(cell), 1);
        }
        return;
    }

    var reg = new RegExp("[1-9]");
    var tempCell, index, tempIndex;

    if (!reg.test(data) || data.length > 1) {
        HighlightCell(cell);

        $(cell).val('');
        return;
    }

    index = cell.match(/(\d+)/)[1];
    tempCell = cell.replace(index, '');
    tempIndex = CheckRules(cell).split(',');

    if (tempIndex.length > 0 && tempIndex[0] != '') {
        for (var i = 0; i < tempIndex.length; i++) {
            HighlightCell(tempCell + tempIndex[i]);
        }
        $(cell).val('');
        return;
    }

    /*if (restorePointArray.length > 0) {
        index = restorePointArray.length - 1;
        if ($(cellArray[index]).val() !== '')
            cellArray[index] += ',';
        cellArray[index] += cell;
    }*/

    if (data !== '') {
        cellArray.push(cell);

        if (holesCount == GetHolesCount(0)) {
            var finalHolesCount = GetFinalHolesCount();
            if (finalHolesCount > 0) {
                GetHolesCount(holesCount - finalHolesCount);
                return;
            }
            else {
                endTime = $.now();
                alert("Puzzle with " + holesCount + " holes solved in " + Math.round((endTime - startTime)/1000) +" seconds");
            }
        }
    }
}

function GetHolesCount(initValue) {
    if (typeof GetHolesCount.count === 'undefined') {
        GetHolesCount.count = 0;
    }
    if (initValue > 0)
        GetHolesCount.count = initValue;
    else
        GetHolesCount.count++;
    return GetHolesCount.count;
}

function GetFinalHolesCount() {
    var count = 0;
    $('#SuDoKuGrid input[type=text]').each(function () {
        if (this.value == '')
            count++;
    });
    return count;
}

function HighlightCell(cell) {
    $(cell).addClass('error').delay(200).queue(function () {
        $(cell).removeClass('error').dequeue();
    });
}

function CheckRules(cell) {
    var cubeLookup = [1, -1, -2, 2, 1, -1];
    var index = cell.match(/(\d+)/)[1].split('');
    var tempIndex = '';
    var row = parseInt(index[0]);
    var column = parseInt(index[1]);
    var tempCell = cell.replace(row + '' + column, '');
    var r, c;
    var data = $(cell).val();
    for (var i = 0; i < 9; i++) {
        if (tempIndex.length > 0 && tempIndex.substr(tempIndex.length - 1) != ',')
            tempIndex += ',';

        if (i != row) {
            if ($(tempCell + i + column).val() == data) {
                tempIndex = tempIndex + i + column;
            }
        }

        if (tempIndex.length > 0 && tempIndex.substr(tempIndex.length - 1) != ',')
            tempIndex += ',';

        if (i != column) {
            if ($(tempCell + row + i).val() == data)
                tempIndex = tempIndex + row + i;
        }

        if (tempIndex.length > 0 && tempIndex.substr(tempIndex.length - 1) != ',')
            tempIndex += ',';


        if (i == column) {
            r = row + cubeLookup[row % 3];
            c = i + cubeLookup[i % 3];
            if ($(tempCell + r + c).val() == data)
                tempIndex = tempIndex + r + c;

            r = row + cubeLookup[(row % 3) + 3];
            if ($(tempCell + r + c).val() == data)
                tempIndex = tempIndex + r + c;
        }

        if (tempIndex.length > 0 && tempIndex.substr(tempIndex.length - 1) != ',')
            tempIndex += ',';

        if (i == row) {
            r = i + cubeLookup[i % 3];
            c = column + cubeLookup[(column % 3) + 3];
            if ($(tempCell + r + c).val() == data)
                tempIndex = tempIndex + r + c;

            r = i + cubeLookup[(i % 3) + 3];
            if ($(tempCell + r + c).val() == data)
                tempIndex = tempIndex + r + c;
        }
    }

    if (tempIndex.substr(tempIndex.length - 1) == ',')
        tempIndex = tempIndex.substr(0, tempIndex.length - 1);

    return tempIndex;
}

function CellInFocus(id) {
    cellInFocus = id;
}

function SetRestorePoint() {
    if (typeof cellInFocus === "undefined")
        return;

    var cell = '#' + cellInFocus;
    if ($(cell).hasClass('restorePoint')) {
        //restorePointArray.splice(restorePointArray.indexOf(cell), 1);
        $(cell).removeClass('restorePoint');
    }
    else {
        //restorePointArray.push(cell);
        $(cell).addClass('restorePoint');
        //if ($(cell).val() !== '') {
        //    cellArray[restorePointArray.length - 1] = cell;
        //}
    }
}

/*function UndoTillRestore() {
    if (typeof cellInFocus === "undefined")
        return;

    var cell = '#' + cellInFocus;
    if ($(cell).hasClass('restorePoint')) {
        restorePointArray.splice(restorePointArray.indexOf(cell), 1);
        $(cell).removeClass('restorePoint');
    }

    if (restorePointArray.length == 0 || cellArray.length == 0)
        return;

    var cells = cellArray[restorePointArray.length - 1].split(',');

    for (var i = 0; i < cells.length; i++) {
        $(cells[i]).val('');
        $(cells[i]).removeClass('restorePoint');
    }

    restorePointArray.splice(restorePointArray.length - 1, 1);
}*/

function UndoTillFlag() {
    if (cellArray !== 'undefined' && cellArray.length > 0) {
        var arrayLength = cellArray.length;
        var cell;
        for (var i = 0; i < arrayLength; i++) {
            cell = cellArray.splice(cellArray.length - 1, 1) + '';
            $(cell).val('');

            if ($(cell).hasClass('restorePoint')) {
                $(cell).removeClass('restorePoint');
                break;
            }
        }
    }
}
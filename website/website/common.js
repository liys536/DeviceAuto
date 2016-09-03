
function get_url() {
    var src = window.location.href;
    var I = src.substr(src.indexOf("?qx=") + 4).length;
    var T = src.indexOf("?qx=");
    //alert(src); 
    if (0 >= T) return "";
    if (I == src.length) return -1;
    var url = src.substr(src.indexOf("?qx=") + 4);
    //alert(url); 
    return url;
}

//调试时正常，发布后出现长日期格式
function myformatter(s) {
    if (!s) return new Date();
    var ss = (s.split('-'));
    var y = parseInt(ss[0], 10);
    var m = parseInt(ss[1], 10);
    var d = parseInt(ss[2], 10);
    if (!isNaN(y) && !isNaN(m) && !isNaN(d)) {
        return y + "-" + m + "-" + d;
    } else {
        return new Date();
        }
}

////调试时出现nan-nan-nan,发布后正常
////日期必须格式化，否则出现无法打开页面的情况
//function myformatter(val) {
//    if (val != null) {
//        var date = new Date(val);
//        return date.getFullYear() + '-' + (date.getMonth() + 1) + '-'
//+ date.getDate();
//        }
//}


function getCurDate() {
    //--------------------------------------------------
    //日期没有格式化，返回样式：2016-6-1
//    var curr_time = new Date();
//    var strDate = curr_time.getFullYear() + "-";
//    var sMonth = curr_time.getMonth() + 1;
//   
//    strDate += sMonth + "-";
//    var sDay = curr_time.getDate();
//    strDate += sDay;

//    return strDate;
    //--------------------------------------------------


    //返回样式：2016-06-01
    var curr_time = new Date().format('yyyy-MM-dd');
    return curr_time;
}

function getCurTime() {
    //返回样式：2016-06-01 05:00:15
    var curr_time = new Date().format('yyyy-MM-dd hh:mm:ss');
    return curr_time;
}


Date.prototype.format = function (format) {
    var date = {
        "M+": this.getMonth() + 1,
        "d+": this.getDate(),
        "h+": this.getHours(),
        "m+": this.getMinutes(),
        "s+": this.getSeconds(),
        "q+": Math.floor((this.getMonth() + 3) / 3),
        "S+": this.getMilliseconds()
    };
    if (/(y+)/i.test(format)) {
        format = format.replace(RegExp.$1,
    (this.getFullYear() + '').substr(4 - RegExp.$1.length));
    }
    for (var k in date) {
        if (new RegExp("(" + k + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? date[k] : ("00" + date[k]).substr(("" + date[k]).length));
        } 
    }

    return format;

 } 

//禁止输入特殊字符
function processSpelChar() {
    var code;
    var character;
    if (document.all) {
        code = window.event.keyCode;
    } else {
        code = arguments.callee.caller.arguments[0].which;
    }
    var character = String.fromCharCode(code);
    var txt = new RegExp(/["=' <>%;*@!~)(&+]/);
    if (txt.test(character)) {
        if (document.all) {
            window.event.returnValue = false;
        } else {
            arguments.callee.caller.arguments[0].preventDefault();
        }
    }
}

function checkEnter(e) {
    var et = e || window.event;
    var keycode = et.charCode || et.keyCode;
    if (keycode == 13) {
        if (window.event)
            window.event.returnValue = false;
        else
            e.preventDefault(); //for firefox
    }

}




//treegrid客户端分页函数，子级节点不统计
function pagerFilter(data) {
    if (data == null) {     //data为空
        data = {
            total: 0,
            rows: 0
        }
    }
    else {      //data非空
        if ($.isArray(data)) {    // is array  
            data = {
                total: data.length,
                rows: data
            }
        }
        var dg = $(this);
        var state = dg.data('treegrid');
        var opts = dg.treegrid('options');
        var pager = dg.treegrid('getPager');
        pager.pagination({
            onSelectPage: function (pageNum, pageSize) {
                opts.pageNumber = pageNum;
                opts.pageSize = pageSize;
                pager.pagination('refresh', {
                    pageNumber: pageNum,
                    pageSize: pageSize
                });
                dg.treegrid('loadData', data);
            }
        });
        if (!data.topRows) {
            data.topRows = [];
            data.childRows = [];
            for (var i = 0; i < data.rows.length; i++) {
                var row = data.rows[i];
                row._parentId ? data.childRows.push(row) : data.topRows.push(row);
            }
            data.total = (data.topRows.length);
        }
        var start = (opts.pageNumber - 1) * parseInt(opts.pageSize);
        var end = start + parseInt(opts.pageSize);
        data.rows = $.extend(true, [], data.topRows.slice(start, end).concat(data.childRows));
    }
    return data;
}


/**
* EasyUI DataGrid根据字段动态合并单元格
* @param fldList 要合并table的id
* @param fldList 要合并的列,用逗号分隔(例如："name,department,office");
*/
function MergeCells(tableID, fldList) {
    var Arr = fldList.split(",");
    var dg = $('#' + tableID);
    var fldName;
    var RowCount = dg.datagrid("getRows").length;
    var span;
    var PerValue = "";
    var CurValue = "";
    var length = Arr.length - 1;
    for (i = length; i >= 0; i--) {
        fldName = Arr[i];
        PerValue = "";
        span = 1;
        for (row = 0; row <= RowCount; row++) {
            if (row == RowCount) {
                CurValue = "";
            }
            else {
                CurValue = dg.datagrid("getRows")[row][fldName];
            }
            if (PerValue == CurValue) {
                span += 1;
            }
            else {
                var index = row - span;
                dg.datagrid('mergeCells', {
                    index: index,
                    field: fldName,
                    rowspan: span,
                    colspan: null
                });
                span = 1;
                PerValue = CurValue;
            }
        }
    }
}




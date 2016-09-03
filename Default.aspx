<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DeviceAuto.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>PDF在线预览</title>
    <script src="Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script src="Scripts/pdfobject.js" type="text/javascript"></script>
    <script type="text/javascript">

        $(function () {
            var w = $(document).width();
            var h = $(document).height();
            $("#pdf1").css("width", w).css("height", h);

            //截取id与上传
            var surl = window.location.href;
            var fname = surl.substring(surl.indexOf("?") + 1, surl.length);
            var sname = fname.substring(4, fname.length);


            // 下面代码都是处理IE浏览器的情况 
            if (window.ActiveXObject || "ActiveXObject" in window) {
                //判断是否为IE浏览器，"ActiveXObject" in window判断是否为IE11
                //判断是否安装了adobe Reader
                for (x = 2; x < 10; x++) {
                    try {
                        oAcro = eval("new ActiveXObject('PDF.PdfCtrl." + x + "');");
                        if (oAcro) {
                            flag = true;
                        }
                    } catch (e) {
                        flag = false;
                    }
                }
                try {
                    oAcro4 = new ActiveXObject('PDF.PdfCtrl.1');
                    if (oAcro4) {
                        flag = true;
                    }
                } catch (e) {
                    flag = false;
                }
                try {
                    oAcro7 = new ActiveXObject('AcroPDF.PDF.1');
                    if (oAcro7) {
                        flag = true;
                    }
                } catch (e) {
                    flag = false;
                }
                if (flag) {
                    $('#pdf1').hide();
                    location = "PDFView.aspx?clj=" + fname;
                }
                else {
                    alert("对不起,您还没有安装PDF阅读器软件,为了方便预览PDF文档,请选择安装！");
                    location = "http://ardownload.adobe.com/pub/adobe/reader/win/9.x/9.3/chs/AdbeRdr930_zh_CN.exe";
                }
            }
            else {

                //alert($.trim(urls));
                var success = new PDFObject({ url: 'UPLoad\\' + sname, pdfOpenParams: { scrollbars: '0', toolbar: '0', statusbar: '0'} }).embed("pdf1");
                if (!success) {
                    var opts = {
                        width: $(document).width(),
                        height: $(document).height(),
                        autoplay: true
                    };

                }
            }
        });

    </script>
</head>
<body>
    <form id="form1" runat="server">    
     <div id="pdf1" >对不起,您还没有安装PDF阅读器软件,为了方便预览PDF文档,请选择安装！
     <a href="http://ardownload.adobe.com/pub/adobe/reader/win/9.x/9.3/chs/AdbeRdr930_zh_CN.exe">AdbeRdr930_zh_CN.exe</a>
     </div>
    
    </form>
</body>
</html>

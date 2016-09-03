<?php
$Line=$_POST['Line'];
//$Line="1号擦手纸";
$Line=iconv('UTF-8','GB2312',$Line);
$conn=odbc_connect('Driver={SQL Server};Server=192.168.1.3;Database=Maintenance_Record',"sa","123") or die("fault");
 $sql="select * from motoraccount where Line='".$Line."'";
 $Line=iconv('GB2312','UTF-8',$Line);
 $res=odbc_exec($conn,$sql) ;
 $json='{"'.$Line.'":{';
 $i=0;
 while(odbc_fetch_row($res))
 {
	 $motorid=odbc_result($res,"DeviceId");
	 $motorname=odbc_result($res,"DeviceName");
	 $motorid=iconv('GB2312','UTF-8',$motorid);
	 $motorname=iconv('GB2312','UTF-8',$motorname);
	 if($i==0)
	 {
	 $json.='"'.$motorid.'":"'.$motorname.'"';
	 $i++;
	 }
	 else
	 $json.=',"'.$motorid.'":"'.$motorname.'"';
	   
	 
 }
 odbc_close($conn);
 $json.='}}';
 echo $json;
?>
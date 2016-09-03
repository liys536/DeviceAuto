<?php
function geterror($conn,$key,$backbearing,$temp,$btemp,$fan,$remark,$time,$remark,$line)
{
	$sql="update MotorRecord set Temp='$temp',Btemp='$btemp',FB='$forebearing',BB='$backbearing',FAN='$fan',remark='$remark' where DeviceId='$key' and Line='$line' and Date='$time'";
	echo $sql;
	odbc_exec($conn,$sql) or odbc_errormsg();
}
?>
<?php
$conn=odbc_connect('Driver={SQL Server};Server=192.168.1.3;Database=Maintenance_Record',"sa","123") or die("fault");
$json=$_POST["json"];
$line=$_POST["line"];

$arr=json_decode($json,TRUE) or die("fault");
$arr_key=array_keys($arr);
$count=count($arr_key);
for($i=0;$i<$count;$i++){

	$key=$arr_key[$i];
	$backbearing=$arr[$key]['backbearing'] ;
	$temp=$arr[$key]['temp'];
	$forebearing=$arr[$key]['forebearing'] ;
	$btemp=$arr[$key]['btemp'];
	$fan=$arr[$key]['fan'];
	$remark=$arr[$key]['remark'];
	$time=date('Y-m-d',time());
	$remark=iconv('UTF-8','GB2312',$arr[$key]['remark']);
	$sql="insert into MotorRecord values('$key','$line','$time','$temp','$btemp','$forebearing','$backbearing','$fan','$remark')";
    odbc_exec($conn,$sql) or geterror($conn,$key,$backbearing,$temp,$btemp,$fan,$remark,$time,$remark,$line); 
	}
 odbc_close($conn);
echo "FILE SEND SUCCESS";
?>
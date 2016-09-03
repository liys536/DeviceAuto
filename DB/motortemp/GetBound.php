<?php include  'CabinetRecord_Library.php';
date_default_timezone_set('Asia/Shanghai');  
?>

<?php
/**********************ÔËÐÐÇø*****************************/
$cabinetid=$_GET["cabinetid"];
$column=$_GET["column"];
$servern=Server;

$coninfo=array("Database"=>Database,"UID"=>USER,"PWD"=>PASSWORD,"CharacterSet"=>"UTF-8");
$conn=sqlsrv_connect($servern,$coninfo);
$stmt = sqlsrv_query($conn,"select b.isx,b.ixx,b.cDjcs,b.iNumber from sbzlb a inner join sbzlb_djcs  b on a.id=isbid  where a.deviceid='".$cabinetid."' and b.inumber='".$column."' ");
$results="";
while($row = sqlsrv_fetch_array($stmt,SQLSRV_FETCH_ASSOC)){ 
	  $results[] = $row; 
 } 

echo json_encode($results, JSON_UNESCAPED_UNICODE), "\n";  
sqlsrv_close($conn);
?>


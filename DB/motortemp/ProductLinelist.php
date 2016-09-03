<?php include  'CabinetRecord_Library.php';
date_default_timezone_set('Asia/Shanghai');  
?>

<?php
/**********************ÔËÐÐÇø*****************************/
$line=$_GET["line"];
$servern=Server;

$coninfo=array("Database"=>Database,"UID"=>USER,"PWD"=>PASSWORD,"CharacterSet"=>"UTF-8");
$conn=sqlsrv_connect($servern,$coninfo);
$stmt = sqlsrv_query($conn,"SELECT djlxb.cDjlx,djlxb_mx.cXh,djlxb_mx.bSet,sbzlb.DeviceName,sbzlb.DeviceId, djlxb.id FROM djlxb INNER JOIN djlxb_mx ON djlxb.id = djlxb_mx.iLxid INNER JOIN sbzlb ON djlxb_mx.iSbid = sbzlb.id where djlxb.cDjlx='".$line."'");
while($row = sqlsrv_fetch_array($stmt,SQLSRV_FETCH_ASSOC)){ 
	  $results[] = $row; 
 } 
echo json_encode($results, JSON_UNESCAPED_UNICODE), "\n";  
sqlsrv_close($conn);
?>
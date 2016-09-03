<?php include  'CabinetRecord_Library.php';
date_default_timezone_set('Asia/Shanghai');  
?>

<?php
/**********************ÔËÐÐÇø*****************************/
$servern=Server;
$coninfo=array("Database"=>Database,"UID"=>USER,"PWD"=>PASSWORD,"CharacterSet"=>"UTF-8");
$conn=sqlsrv_connect($servern,$coninfo);
$stmt = sqlsrv_query($conn,"select TypeName from CabinetRecordType");
$results = array();
while($row = sqlsrv_fetch_array($stmt,SQLSRV_FETCH_ASSOC)){ 
	  $results[] = $row; 
 } 
echo json_encode($results, JSON_UNESCAPED_UNICODE), "\n";  
sqlsrv_close($conn);
?>
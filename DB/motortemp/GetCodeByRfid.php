<?php include  'CabinetRecord_Library.php';
date_default_timezone_set('Asia/Shanghai');  
?>

<?php
/**********************ÔËÐÐÇø*****************************/
		$rfid=$_GET["rfid"];
		$servern=Server;
		$coninfo=array("Database"=>Database,"UID"=>USER,"PWD"=>PASSWORD,"CharacterSet"=>"UTF-8");
		$conn = sqlsrv_connect($servern, $coninfo);
		$query = sqlsrv_query($conn, "select DeviceId from sbzlb where cSbsbh='".$rfid."'");
		
		while( $row = sqlsrv_fetch_array( $query, SQLSRV_FETCH_ASSOC))
		{
			echo $row['DeviceId']."<br>\n";
		}
?>
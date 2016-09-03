<?php
		$serverName = "127.0.0.1";
		$connectionInfo = array( "UID"=>"sa","PWD"=>"123","Database"=>"Basic_Information","CharacterSet"=>"UTF-8"); // 这一行是重点
		$conn = sqlsrv_connect($serverName, $connectionInfo);
		$query = sqlsrv_query($conn, "select * from employee_information where ID='049718' and password='7515616'");
		while( $row = sqlsrv_fetch_array( $query, SQLSRV_FETCH_ASSOC))
		{
			echo $row['name']."<br>\n";
		}
?>



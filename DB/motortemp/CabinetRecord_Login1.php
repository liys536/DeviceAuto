<?php
		$id=$_GET["id"];
		$passowrd=$_GET["pas"];
		
		
		$serverName = "127.0.0.1";
		$connectionInfo = array( "UID"=>"sa","PWD"=>"123","Database"=>"Basic_Information","CharacterSet"=>"UTF-8"); // 这一行是重点
		$conn = sqlsrv_connect($serverName, $connectionInfo);
		$query = sqlsrv_query($conn, "select * from employee_information where ID='$id' and password='$passowrd'");
		while( $row = sqlsrv_fetch_array( $query, SQLSRV_FETCH_ASSOC))
		{
			echo $row['name'];
		}
?>
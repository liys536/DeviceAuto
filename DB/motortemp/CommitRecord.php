<?php include  'CabinetRecord_Library.php';
date_default_timezone_set('Asia/Shanghai');  
?>

<?php
/**********************ÔËÐÐÇø*****************************/
$data=$_POST["data"];
//$con=odbc_connect(CONNECTIONSTRING,USER,PASSWORD);
//odbc_exec($con,$data);
//odbc_close($con);
$servern=Server;
  $coninfo=array("Database"=>Database,"UID"=>USER,"PWD"=>PASSWORD);
  $conn=sqlsrv_connect($servern,$coninfo);
  if(sqlsrv_query($conn,iconv('utf-8', 'GB2312//IGNORE', $data)))
{
      echo "FILE SEND SUCCESS";
} 
else
{
      echo "Error in statement execution";
      die( print_r( sqlsrv_errors(), true));
}
  sqlsrv_close($conn); 
?>

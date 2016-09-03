<?php include  'CabinetRecord_Library.php';
date_default_timezone_set('Asia/Shanghai');  
?>

<?php
/**********************ÔËÐÐÇø*****************************/
$column=$_GET["column"];
$typename=$_GET["typename"];
$DeviceId=$_GET["DeviceId"];
$startDate=$_GET["startDate"];
$endDate=$_GET["endDate"];
$servern=Server;

$coninfo=array("Database"=>Database,"UID"=>USER,"PWD"=>PASSWORD,"CharacterSet"=>"UTF-8");
$conn=sqlsrv_connect($servern,$coninfo);
if($DeviceId==''||$DeviceId==NUll){
	$stmt = sqlsrv_query($conn,"select ".$column." from (select sb.DeviceId,sb.DeviceName,sb.line,sb.belong,sb.iDjid,dj.* from djjlb dj inner join sbzlb sb on dj.iSbid = sb.id) sbdj inner join CabinetRecordType cr on sbdj.iDjid=cr.id where cr.TypeName='".$typename."' and sbdj.dRq>='".$startDate."'  order by sbdj.dRq ");
}else if($startDate==''||$startDate==NUll){
	$stmt = sqlsrv_query($conn,"select ".$column." from (select sb.DeviceId,sb.DeviceName,sb.line,sb.belong,sb.iDjid,dj.* from djjlb dj inner join sbzlb sb on dj.iSbid = sb.id) sbdj inner join CabinetRecordType cr on sbdj.iDjid=cr.id where cr.TypeName='".$typename."' and sbdj.DeviceId='".$DeviceId."' order by sbdj.dRq ");
}else{
	$stmt = sqlsrv_query($conn,"select ".$column." from (select sb.DeviceId,sb.DeviceName,sb.line,sb.belong,sb.iDjid,dj.* from djjlb dj inner join sbzlb sb on dj.iSbid = sb.id) sbdj inner join CabinetRecordType cr on sbdj.iDjid=cr.id where cr.TypeName='".$typename."' and sbdj.DeviceId='".$DeviceId."' and sbdj.dRq>='".$startDate."'  order by sbdj.dRq ");
}

$results = array();
$file=fopen("msg.txt","w");
fwrite($file,$typename);
fclose($file);
while($row = sqlsrv_fetch_array($stmt,SQLSRV_FETCH_ASSOC)){ 
	  $results[] = $row; 
 } 
function url_encode($str) {  
    if(is_array($str)) {  
        foreach($str as $key=>$value) {  
            $str[urlencode($key)] = url_encode($value);  
        }  
    } else {  
	//$name=iconv('utf-8','GB2312',$name);
//$file=fopen("msg.txt","w");
//fwrite($file,$str);
        $str = urlencode($str);  
    }  
    return $str;  
}  
echo json_encode($results, JSON_UNESCAPED_UNICODE), "\n";  
//echo urldecode(json_encode(url_encode($results)));    
sqlsrv_close($conn);
?>
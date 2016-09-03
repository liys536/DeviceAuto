<?php include  'CabinetRecord_Library.php';
date_default_timezone_set('Asia/Shanghai');  
?>

<?php
/**********************ÔËÐÐÇø*****************************/
$DeviceId=$_GET["DeviceId"];
$servern=Server;

$coninfo=array("Database"=>Database,"UID"=>USER,"PWD"=>PASSWORD,"CharacterSet"=>"UTF-8");
$conn=sqlsrv_connect($servern,$coninfo);
	$stmt = sqlsrv_query($conn,"select c.*,d.TypeName from (select a.DeviceId,a.DeviceName,b.cSbfl,a.cSbsbh,a.Line,a.belong,a.csbxh,a.cJscs,a.cSccj,a.iZjsl,a.cAzdd,a.dTyrq,a.iDjid from sbzlb a left join sbflb b on a.iLbid=b.id) c left join CabinetRecordType d on c.iDjid=d.id");
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
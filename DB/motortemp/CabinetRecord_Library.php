<?php
   /*******************************参数区*********************************************/
define("CONNECTIONSTRING",'Driver={SQL Server};Server=127.0.0.1;Database=DeviceAuto');
define("PASSWORD",'123');
define("USER",'sa');
define("Server",'127.0.0.1');
define("Database",'DeviceAuto');
define("CabinetRecord","djjlb");
define("CabinetNeedRecord","sbzlb");
define("CabinetRecordType","CabinetRecordType");
?>
<?php
/****************连接数据库********************/
abstract class SqlConnection 
{
	private $con;
	abstract protected function GetData($con);
    function SqlConnection( $connection,$user,$key)
	{
		$con=odbc_connect($connection,$user,$key);
		$this->GetData($con);
		odbc_close($con);
	}
	
}
class ParseXML{
	private $dom;
	private $typenodes,$cabinetnodes;
	private $typenode,$cabinetnode;
	private $currenttypeitem,$currentcabinetitem;
	public $SQL;
	private $name;
	function ParseXML($xmlstring,$name)
	{
           
		//$this->name=iconv('UTF-8','GB2312',$name);
                $this->name=iconv('utf-8', 'GB2312//IGNORE', $name);
                $this->name=str_replace("??","",$this->name);
	        // $this->name=$name;
		$this->dom=new DOMDocument();
		$this->dom->loadXML($xmlstring);	
		$this->cabinetnodes=$this->dom->getElementsByTagName("Cabinet");
		
	}
	public function Reset()
	{
		$this->currenttypeitem=0;
		$this->currentcabinetitem=0;
	}
	public function GetNextCabinet()
	{
		 if($this->currentcabinetitem>=$this->cabinetnodes->length)
		     return false;
		 else 
		 {
			 $this->cabinetnode=$this->cabinetnodes->item($this->currentcabinetitem++);
			 $this->typenode=$this->cabinetnode->parentNode;
			 return true;
		 }
	}
	public function GetResult(){
		$file=fopen("msg.txt","w");
	    fwrite($file,$this->GetSQL());
	    fclose($file);
		return "insert into CabinetRecord(CabinetID,CabinetName,Line,belong,Date,TypeName,Name,REAL4,REAL2,REAL3,REAL1,BOOL3,BOOL2,BOOL1,STRING1) values('AA1302','电容柜','TM10','电气','2015-05-21 09:11:55','自动化测试用','刘永君','111','111','111','123','1','1','1','')";
	}
	public function GetSQL()
	{
		$name="";
		$value="";
		$date=date('Y-m-d H:i:s',time());
		$colnodes=$this->cabinetnode->childNodes;
		$attr=$this->typenode->attributes->getNamedItem("TypeName");
		$typename=iconv('UTF-8','GB2312',$attr->textContent);
		$attr=$this->cabinetnode->attributes->getNamedItem("CabinetId");
		$cabinetid=iconv('UTF-8','GB2312',$attr->textContent);
		
		$attr=$this->cabinetnode->attributes->getNamedItem("CabinetName");
		$cabinetname=iconv('UTF-8','GB2312',$attr->textContent);
		
		$attr=$this->cabinetnode->attributes->getNamedItem("Line");
		$line=iconv('UTF-8','GB2312',$attr->textContent);
				
		$attr=$this->cabinetnode->attributes->getNamedItem("belong");
		$belong=iconv('UTF-8','GB2312',$attr->textContent);
		
		$name="iSbid,dRq,cDjr";
		
		$con=odbc_connect(CONNECTIONSTRING,USER,PASSWORD);
		$res=odbc_exec($con,"select * from sbzlb where DeviceId='".$cabinetid."' ");
	    $value="'".odbc_result($res,"id")."','".$date."','".str_replace("??","",$this->name)."'";
		odbc_close($con);
		foreach($colnodes as $node)
		{
			$colname=iconv('UTF-8','GB2312',$node->nodeName);
			$colvalue=iconv('UTF-8','GB2312',$node->textContent);
                        //if(stristr($colname,"STRING")!=false)
                        //{
                            //$colvalue=str_replace("'"."''",$colvalue);
                       // }
			$name.=",".$colname;
			$value.=",'".$colvalue."'";
		}
	        $Sql="insert into djjlb(".$name.") values(".$value.")";
                //$file=fopen("msg.txt","w");
	        //fwrite($file,$Sql);
	         //fclose($file);
		return $Sql;
                
                 //return  "";
	}

}
?>
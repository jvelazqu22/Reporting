The iBank.Repository.SQL data repositories expect certain connection strings dependent on environments. 
Please add the below connection strings to the relevant locations within your project.

##### LOCAL DEBUG #####
<add name="iBankMastersEntities" connectionString="data source=192.168.14.121;initial catalog=ibankmasters;user id=<user>;password=<password>;MultipleActiveResultSets=True;App=EntityFramework" providerName="System.Data.SqlClient" />
<add name="CISMasterEntities" connectionString="data source=192.168.14.121;initial catalog=CISMaster;user id=<user>;password=<password>;MultipleActiveResultSets=True;App=EntityFramework" providerName="System.Data.SqlClient" />
<add name="iBankAdministrationEntities" connectionString="data source=192.168.14.121;initial catalog=ibankadministration;user id=<user>;password=<password>;MultipleActiveResultSets=True;App=EntityFramework" providerName="System.Data.SqlClient" />
<add name="FormattableConnectionString" connectionString="data source={0};initial catalog={1};user id=<user>;password=<password>;MultipleActiveResultSets=True;App=EntityFramework" providerName="System.Data.SqlClient" />


##### KEYSTONE #####
<add name="iBankMastersEntities" connectionString="data source=192.168.14.121;initial catalog=ibankmasters;Integrated Security=True;MultipleActiveResultSets=True;App=EntityFramework;Connection Timeout=90" providerName="System.Data.SqlClient" />
<add name="CISMasterEntities" connectionString="data source=192.168.14.121;initial catalog=CISMaster;Integrated Security=True;MultipleActiveResultSets=True;App=EntityFramework;Connection Timeout=90" providerName="System.Data.SqlClient" />
<add name="iBankAdministrationEntities" connectionString="data source=192.168.14.121;initial catalog=ibankadministration;Integrated Security=True;MultipleActiveResultSets=True;App=EntityFramework;Connection Timeout=90" providerName="System.Data.SqlClient" />
<add name="FormattableConnectionString" connectionString="data source={0};initial catalog={1};Integrated Security=True;MultipleActiveResultSets=True;App=EntityFramework;Connection Timeout=90" providerName="System.Data.SqlClient" />


##### PRODUCTION #####
<add name="iBankMastersEntities" connectionString="data source=ibanksql1;initial catalog=ibankmasters;Integrated Security=True;MultipleActiveResultSets=True;App=EntityFramework;Connection Timeout=90" providerName="System.Data.SqlClient" />
<add name="CISMasterEntities" connectionString="data source=ibanksql1;initial catalog=CISMaster;Integrated Security=True;MultipleActiveResultSets=True;App=EntityFramework;Connection Timeout=90" providerName="System.Data.SqlClient" />
<add name="iBankAdministrationEntities" connectionString="data source=systemssql01;initial catalog=ibankadministration;Integrated Security=True;MultipleActiveResultSets=True;App=EntityFramework;Connection Timeout=90" providerName="System.Data.SqlClient" />
<add name="FormattableConnectionString" connectionString="data source={0};initial catalog={1};Integrated Security=True;MultipleActiveResultSets=True;App=EntityFramework;Connection Timeout=90" providerName="System.Data.SqlClient" />

##### PRODUCTION GSA #####
<add name="iBankMastersEntities" connectionString="data source=gsasql01;initial catalog=ibankmasters;Integrated Security=True;MultipleActiveResultSets=True;App=EntityFramework;Connection Timeout=90" providerName="System.Data.SqlClient" />
<add name="CISMasterEntities" connectionString="data source=gsasql01;initial catalog=CISMaster;Integrated Security=True;MultipleActiveResultSets=True;App=EntityFramework;Connection Timeout=90" providerName="System.Data.SqlClient" />
<add name="iBankAdministrationEntities" connectionString="data source=gsasql01;initial catalog=ibankadministration;Integrated Security=True;MultipleActiveResultSets=True;App=EntityFramework;Connection Timeout=90" providerName="System.Data.SqlClient" />
<add name="FormattableConnectionString" connectionString="data source={0};initial catalog={1};Integrated Security=True;MultipleActiveResultSets=True;App=EntityFramework;Connection Timeout=90" providerName="System.Data.SqlClient" />

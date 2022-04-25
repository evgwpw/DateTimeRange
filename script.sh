read -p 'дата начала: ' begin
read -p 'дата окончания: ' end
read -p 'задача: ' task

find /u01/crafttalk/packages/logs/botmediator/arch/ -name "BotMediator.Debug.*" -type f -newermt "$begin" ! -newermt "$end" -exec cp '{}' /u01/install/Logs/botmediator/ \;
find /u01/crafttalk/packages/logs/botmediator/arch/ -name "BotMediator.Info..*" -type f -newermt "$begin" ! -newermt "$end" -exec cp '{}' /u01/install/Logs/botmediator/ \;
find /u01/crafttalk/packages/logs/botmediator/arch/ -name "BotMediator.Warm..*" -type f -newermt "$begin" ! -newermt "$end" -exec cp '{}' /u01/install/Logs/botmediator/ \;
find /u01/crafttalk/packages/logs/botmediator/arch/ -name "BotMediator.Error.*" -type f -newermt "$begin" ! -newermt "$end" -exec cp '{}' /u01/install/Logs/botmediator/ \;


find /u01/crafttalk/packages/logs/agents/arch/ -name "Agents.Debug.*" -type f -newermt "$begin" ! -newermt "$end" -exec cp '{}' /u01/install/Logs/agents/ \;
find /u01/crafttalk/packages/logs/agents/arch/ -name "Agents.Info.*" -type f -newermt "$begin" ! -newermt "$end" -exec cp '{}' /u01/install/Logs/agents/ \;
find /u01/crafttalk/packages/logs/agents/arch/ -name "Agents.Warm.*" -type f -newermt "$begin" ! -newermt "$end" -exec cp '{}' /u01/install/Logs/agents/ \;
find /u01/crafttalk/packages/logs/agents/arch/ -name "Agents.Error.*" -type f -newermt "$begin" ! -newermt "$end" -exec cp '{}' /u01/install/Logs/agents/ \;


find /u01/crafttalk/packages/logs/workplace/arch/ -name "Workplace.Debug.*" -type f -newermt "$begin" ! -newermt "$end" -exec cp '{}' /u01/install/Logs/workplace/ \;
find /u01/crafttalk/packages/logs/workplace/arch/ -name "Workplace.Info.*" -type f -newermt "$begin" ! -newermt "$end" -exec cp '{}' /u01/install/Logs/workplace/ \;
find /u01/crafttalk/packages/logs/workplace/arch/ -name "Workplace.Warm.*" -type f -newermt "$begin" ! -newermt "$end" -exec cp '{}' /u01/install/Logs/workplace/ \;
find /u01/crafttalk/packages/logs/workplace/arch/ -name "Workplace.Error.*" -type f -newermt "$begin" ! -newermt "$end" -exec cp '{}' /u01/install/Logs/workplace/ \;


find /u01/crafttalk/packages/logs/siebelintegration/arch/ -name "SiebelIntegration.Debug.*" -type f -newermt "$begin" ! -newermt "$end" -exec cp '{}' /u01/install/Logs/siebelintegration/ \;
find /u01/crafttalk/packages/logs/siebelintegration/arch/ -name "SiebelIntegration.Info.*" -type f -newermt "$begin" ! -newermt "$end" -exec cp '{}' /u01/install/Logs/siebelintegration/ \;
find /u01/crafttalk/packages/logs/siebelintegration/arch/ -name "SiebelIntegration.Warm.*" -type f -newermt "$begin" ! -newermt "$end" -exec cp '{}' /u01/install/Logs/siebelintegration/ \;
find /u01/crafttalk/packages/logs/siebelintegration/arch/ -name "SiebelIntegration.Error.*" -type f -newermt "$begin" ! -newermt "$end" -exec cp '{}' /u01/install/Logs/siebelintegration/ \;


find /u01/crafttalk/packages/logs/channels/arch/ -name "ChannelsService.Host.Debug.*" -type f -newermt "$begin" ! -newermt "$end" -exec cp '{}' /u01/install/Logs/Channels/ \;
find /u01/crafttalk/packages/logs/channels/arch/ -name "ChannelsService.Host.Info.*" -type f -newermt "$begin" ! -newermt "$end" -exec cp '{}' /u01/install/Logs/Channels/ \;
find /u01/crafttalk/packages/logs/channels/arch/ -name "ChannelsService.Warm.*" -type f -newermt "$begin" ! -newermt "$end" -exec cp '{}' /u01/install/Logs/Channels/ \;
find /u01/crafttalk/packages/logs/channels/arch/ -name "ChannelsService.Host.Error.*" -type f -newermt "$begin" ! -newermt "$end" -exec cp '{}' /u01/install/Logs/Channels/ \;


tar cvzf /u01/install/{$task}.tar.gz /u01/install/Logs

echo "logs is OK"

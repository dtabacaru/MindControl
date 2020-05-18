local THREE_BYTE_MAX_VAL = 16777215;
local PI_MATH = 3.141592654;

local errorUpdateCount = 0;
local errorFlag = false;
local playerCasting = false;
local setKeyFlag = false;
local whispUpdateCount = 0;
local whisperFlag = false;
local startFlag = false;
local debugFlag = false;
local debugText = "";

function VisionControlData_OnLoad()
	
end

local function OR(a,b)
    local p = 1;
	local c = 0;
	
    while a + b > 0 do
        local ra = mod(a,2);
		local rb = mod(b,2);
		
        if ra + rb > 0 then
			c = c + p;
		end
		
        a = (a - ra)/2;
		b = (b - rb)/2;
		p = p * 2;
    end
	
    return c
end

local function AND(a,b)
    local p = 1;
	local c = 0;
    
	while a > 0 and b > 0 do
        local ra = mod(a,2);
		local rb = mod(b,2);
        
		if ra + rb > 1 then 
			c = c + p;
		end
        
		a = (a - ra)/2;
		b = (b - rb)/2;
		p = p * 2;
    end
    
	return c;
end

local function RBS(a, b)
	return math.floor(a/(2^b));
end

local function IntegerRound(floatValue)
	local temp = math.floor(floatValue);
	
	if floatValue - temp >= 0.5 then
		return math.floor(floatValue+0.5)
	else
		return temp;
	end
	
end

local function NOT(n)
    local p = 1;
	local c = 0;
    
	while n > 0 do
        local r = mod(n,2);
		
        if r < 1 then
			c = c + p;
		end
        
		n = (n-r)/2;
		p = p * 2;
    end
	
    return c
end

local function SetTextDebug(text)
	if debugFlag == true then
		DebugText:SetText(text);
	else
		DebugText:SetText("");
	end
end

local function ThreeByteThreePixel(pixelValue,Pixel1,Pixel2,Pixel3)
	local pixelIntValue = IntegerRound(pixelValue * THREE_BYTE_MAX_VAL);
	
	local byte2 = AND(RBS(pixelIntValue,16),255);
	local byte1 = AND(RBS(pixelIntValue,8),255);
	local byte0 = AND(pixelIntValue,255);

	Pixel1:SetColorTexture(byte0/255,0,0,1);
	Pixel2:SetColorTexture(0,byte1/255,0,1);
	Pixel3:SetColorTexture(0,0,byte2/255,1);
end

local function CheckGCD(checkName)

	local i = 1;
	while true do
	   local spellName, spellRank = GetSpellInfo(i, BOOKTYPE_SPELL)
	   
	   if not spellName then
		  do break end
	   end
	   
	   if spellName == checkName then
	      
		  	local start, duration, enabled, modRate = GetSpellCooldown(i,BOOKTYPE_SPELL);
	
			if duration > 0 then
				UseSkillPixel:SetColorTexture(0, 0, 0, 1);
				return "GCD: False" .. "\n";
			else
				UseSkillPixel:SetColorTexture(1, 1, 1, 1);
				return "GCD: True" .. "\n";
			end
		  
	   end
	   
	   i = i + 1
	end

	UseSkillPixel:SetColorTexture(0, 0, 0, 1);
	return "GCD: False" .. "\n";
end

local function ThreeByteOnePixel(pixelValue,Pixel)
	local pixelIntValue = IntegerRound(pixelValue);
	
	local byte2 = AND(RBS(pixelIntValue,16),255);
	local byte1 = AND(RBS(pixelIntValue,8),255);
	local byte0 = AND(pixelIntValue,255);
	
	Pixel:SetColorTexture(byte0/255, byte1/255, byte2/255, 1);
end

function VisionControlData_OnUpdate()

	if setKeyFlag == false then
		SetBindingClick("CTRL-Q", VisionControlStartButton:GetName());
		setKeyFlag = true;
	end

	local debugString = ""

	FindPixel1:SetColorTexture(50/255, 100/255, 150/255, 1);
	FindPixel2:SetColorTexture(150/255, 50/255, 100/255, 1);

	local map = C_Map.GetBestMapForUnit("player")
	
	if map == nil then 
		ThreeByteThreePixel(0,XPositionR,XPositionG,XPositionB)
		ThreeByteThreePixel(0,YPositionR,YPositionG,YPositionB)
		ThreeByteOnePixel(0,MapIdPixel);
		
		debugString = debugString .. "Map: 0\n";
		debugString = debugString .. "X: 0\n";
		debugString = debugString .. "Y: 0\n";
	else
		local position = C_Map.GetPlayerMapPosition(map, "player")
	
		ThreeByteThreePixel(position.x,XPositionR,XPositionG,XPositionB)
		ThreeByteThreePixel(position.y,YPositionR,YPositionG,YPositionB)
		ThreeByteOnePixel(map,MapIdPixel);
		
		debugString = debugString .. "Map: " .. map .. "\n";
		debugString = debugString .. "X: " .. position.x*100 .. "\n";
		debugString = debugString .. "Y: " .. position.y*100 .. "\n";
	end
	
	local heading = GetPlayerFacing();
	
	if heading == nil then
		HeadingPixelR:SetColorTexture(0,0,0,1);
		HeadingPixelG:SetColorTexture(0,0,0,1);
		HeadingPixelB:SetColorTexture(0,0,0,1);
		
		ThreeByteThreePixel(0,HeadingPixelR,HeadingPixelG,HeadingPixelB);
	
		debugString = debugString .. "Heading: 0\n";
	else
		ThreeByteThreePixel(heading/(2*PI_MATH),HeadingPixelR,HeadingPixelG,HeadingPixelB);
	
		debugString = debugString .. "Heading: " .. heading*(180/PI_MATH) .. "\n";
	end

	
	if CheckInteractDistance("target", 4) then
		FarRangePixel:SetColorTexture(1, 1, 1, 1);
		debugString = debugString .. "Far: True" .. "\n";
	else
		FarRangePixel:SetColorTexture(0, 0, 0, 1);
		debugString = debugString .. "Far: False" .. "\n";
	end
	
	if CheckInteractDistance("target", 2) then
		MediumRangePixel:SetColorTexture(1, 1, 1, 1);
		debugString = debugString .. "Medium: True" .. "\n";
	else
		MediumRangePixel:SetColorTexture(0, 0, 0, 1);
		debugString = debugString .. "Medium: False" .. "\n";
	end
	
	if CheckInteractDistance("target", 3) then
		CloseRangePixel:SetColorTexture(1, 1, 1, 1);
		debugString = debugString .. "Close: True" .. "\n";
	else
		CloseRangePixel:SetColorTexture(0, 0, 0, 1);
		debugString = debugString .. "Close: False" .. "\n";
	end
	
	local playerMaxHealth = UnitHealthMax("player");
	ThreeByteOnePixel(playerMaxHealth,PlayerMaxHealthPixel);
	
	debugString = debugString .. "Max HP: " .. playerMaxHealth .. "\n";
	
	local playerHealth = UnitHealth("player");
	ThreeByteOnePixel(playerHealth,PlayerHealthPixel);
	
	debugString = debugString .. "HP: " .. playerHealth .. "\n";
	
	local playerMaxMana = UnitPowerMax("player");
	ThreeByteOnePixel(playerMaxMana,PlayerMaxManaPixel);
	
	debugString = debugString .. "Max MP: " .. playerMaxMana .. "\n";
	
	local playerMana = UnitPower("player");
	ThreeByteOnePixel(playerMana,PlayerManaPixel);
	
	debugString = debugString .. "MP: " .. playerMana .. "\n";
	
	if UnitExists("target") then
		HasTargetPixel:SetColorTexture(1, 1, 1, 1);
		debugString = debugString .. "Has Target: True" .. "\n";
	else
		HasTargetPixel:SetColorTexture(0, 0, 0, 1);
		debugString = debugString .. "Has Target: False" .. "\n";
	end
	
	local targetHealth = UnitHealth("target");
	ThreeByteOnePixel(targetHealth,TargetHealthPixel);
	
	debugString = debugString .. "Target HP: " .. targetHealth .. "\n";
	
	local targetMana = UnitPower("target");
	ThreeByteOnePixel(targetMana,TargetManaPixel);
	
	debugString = debugString .. "Target MP: " .. targetMana .. "\n";

	local targetMaxHealth = UnitHealthMax("target");
	ThreeByteOnePixel(targetMaxHealth,TargetMaxHealthPixel);
	
	debugString = debugString .. "Target Max HP: " .. targetMaxHealth .. "\n";
	
	local targetMaxtMana = UnitPowerMax("target");
	ThreeByteOnePixel(targetMaxtMana,TargetMaxManaPixel);
	
	debugString = debugString .. "Target Max MP: " .. targetMaxtMana .. "\n";
	
	if UnitAffectingCombat("player") then
		CombatPixel:SetColorTexture(1, 1, 1, 1);
		debugString = debugString .. "In Combat: True" .. "\n";
	else
		CombatPixel:SetColorTexture(0, 0, 0, 1);
		debugString = debugString .. "In Combat: False" .. "\n";
	end
	
	if SpellCanTargetUnit("target") then
		SpellCanAttackTargetPixel:SetColorTexture(1, 1, 1, 1);
		debugString = debugString .. "Can Spell Attack: True" .. "\n";
	else
		SpellCanAttackTargetPixel:SetColorTexture(0, 0, 0, 1);
		debugString = debugString .. "Can Spell Attack: False" .. "\n";
	end
	
		if UnitCanAttack("player", "target") then
		CanAttackPixel:SetColorTexture(1, 1, 1, 1);
		debugString = debugString .. "Can Attack: True" .. "\n";
	else
		CanAttackPixel:SetColorTexture(0, 0, 0, 1);
		debugString = debugString .. "Can Attack: False" .. "\n";
	end
	
	if UnitIsDead("target") then
		TargetIsDeadPixel:SetColorTexture(1, 1, 1, 1);
		debugString = debugString .. "Dead: True" .. "\n";
	else
		TargetIsDeadPixel:SetColorTexture(0, 0, 0, 1);
		debugString = debugString .. "Dead: False" .. "\n";
	end
	
	if UnitClassification("target") == "elite" then
		TargetIsElitePixel:SetColorTexture(1, 1, 1, 1);
		debugString = debugString .. "Elite: True" .. "\n";
	else
		TargetIsElitePixel:SetColorTexture(0, 0, 0, 1);
		debugString = debugString .. "Elite: False" .. "\n";
	end
	
	reaction = UnitReaction("player", "target")
	if reaction == nil then
		ThreeByteOnePixel(0,TargetReactionPixel);
		debugString = debugString .. "Reaction: 0" .. "\n";
	else
		ThreeByteOnePixel(reaction,TargetReactionPixel);
		debugString = debugString .. "Reaction: " .. reaction .. "\n";
	end
	
	targetLevel = UnitLevel("target");
	ThreeByteOnePixel(targetLevel,TargetLevelPixel);
	
	debugString = debugString .. "Target Level: " .. targetLevel .. "\n";
	
	playerLevel = UnitLevel("player");
	ThreeByteOnePixel(playerLevel,PlayerLevelPixel);
	
	debugString = debugString .. "Player Level: " .. playerLevel .. "\n";
	
	if UnitIsEnemy("player","target") then
		TargetIsEnemyPixel:SetColorTexture(1, 1, 1, 1);
		debugString = debugString .. "Is Enemy: True" .. "\n";
	else
		TargetIsEnemyPixel:SetColorTexture(0, 0, 0, 1);
		debugString = debugString .. "Is Enemy: False" .. "\n";
	end
	
	local targetFaction = UnitFactionGroup("target");
	if targetFaction == nil then
		ThreeByteOnePixel(0,TargetFactionPixel);
		debugString = debugString .. "Target Faction: 0" .. "\n";
	elseif targetFaction == "Alliance" then
		ThreeByteOnePixel(1,TargetFactionPixel);
		debugString = debugString .. "Target Faction: 1" .. "\n";
	elseif targetFaction == "Horde" then
		ThreeByteOnePixel(2,TargetFactionPixel);
		debugString = debugString .. "Target Faction: 2" .. "\n";
	end
	
	local comboPoints = GetComboPoints("player", "target");
	ThreeByteOnePixel(comboPoints,TargetComboPointsPixel);	
	
	debugString = debugString .. "Combo Points: " .. comboPoints .. "\n";
	
	local ammoCount = GetInventoryItemCount("player", INVSLOT_RANGED );
	ThreeByteOnePixel(ammoCount,AmmoCountPixel);	
	
	debugString = debugString .. "Ammo: " .. ammoCount .. "\n";
	
	if UnitIsPlayer("target") then
		IsPlayerPixel:SetColorTexture(1, 1, 1, 1);
		debugString = debugString .. "Is Player: True" .. "\n";
	else
		IsPlayerPixel:SetColorTexture(0, 0, 0, 1);
		debugString = debugString .. "Is Player: False" .. "\n";
	end	
	
	if UnitAffectingCombat("target") then
		TargetCombatPixel:SetColorTexture(1, 1, 1, 1);
		debugString = debugString .. "Target Combat: True" .. "\n";
	else
		TargetCombatPixel:SetColorTexture(0, 0, 0, 1);
		debugString = debugString .. "Target Combat: False" .. "\n";
	end	
	
	if UnitIsDead("player") then
		DeadPixel:SetColorTexture(1, 1, 1, 1);
		debugString = debugString .. "Dead: True" .. "\n";
	else
		DeadPixel:SetColorTexture(0, 0, 0, 1);
		debugString = debugString .."Dead: False" .. "\n";
	end
	
	if startFlag == true then
		StartPixel:SetColorTexture(1/255, 255/255, 2/255, 1);
		StartStopIndicator:SetColorTexture(0, 1, 0, 0.2);
	else
		StartPixel:SetColorTexture(1, 0, 0, 1);
		StartStopIndicator:SetColorTexture(1, 0, 0, 0.2);
	end
	
	if IsCurrentAction(1)   or 
	   IsCurrentAction(73)  or
	   IsCurrentAction(85)  or 
	   IsCurrentAction(97)  or
	   IsCurrentAction(109) then
		AttackingPixel:SetColorTexture(1, 1, 1, 1);
		debugString = debugString .. "Attacking: True" .. "\n";
	else
		AttackingPixel:SetColorTexture(0, 0, 0, 1);
		debugString = debugString .. "Attacking: False" .. "\n";
	end
	
	local targetName = UnitName("target")
	
	-- Pixel array
	TargetNamePixels = {};
	TargetNamePixels[0] = TargetNamePixel1;
	TargetNamePixels[1] = TargetNamePixel2;
	TargetNamePixels[2] = TargetNamePixel3;
	TargetNamePixels[3] = TargetNamePixel4;
	TargetNamePixels[4] = TargetNamePixel5;
	TargetNamePixels[5] = TargetNamePixel6;
	TargetNamePixels[6] = TargetNamePixel7;
	TargetNamePixels[7] = TargetNamePixel8;
	TargetNamePixels[8] = TargetNamePixel9;
	TargetNamePixels[9] = TargetNamePixel10;
	
	-- Reset pixels
	for i = 0, 9 do
		TargetNamePixels[i]:SetColorTexture(0, 0, 0, 1);
	end

	if targetName == nil then
		debugString = debugString .. "Target: " .. "\n";
	else
		debugString = debugString .. "Target: " .. UnitName("target") .. "\n";

		local nameLen = string.len(targetName);
		
		local byteCount = 1;
		
		for i = 0, 9 do
			
			if byteCount > nameLen then break end;
			
			local val1 = string.byte(string.sub(targetName,byteCount,byteCount));
			if val1 == nil then
				break;
			else
				TargetNamePixels[i]:SetColorTexture(val1/255, 0, 0, 1);
			end
			byteCount = byteCount + 1;
		
			local val2 = string.byte(string.sub(targetName,byteCount,byteCount));
			if val2 == nil then
				break;
			else
				TargetNamePixels[i]:SetColorTexture(val1/255, val2/255, 0, 1);
			end
			byteCount = byteCount + 1;
		
			local val3 = string.byte(string.sub(targetName,byteCount,byteCount));
			if val3 == nil then
				break;
			else
				TargetNamePixels[i]:SetColorTexture(val1/255, val2/255, val3/255, 1);
			end
			byteCount = byteCount + 1;
			
		end
		
	end
	
	if errorFlag == true then
	
		if errorUpdateCount == 4 then
			errorUpdateCount = 0;
			ErrorPixel:SetColorTexture(0, 0, 0, 1);
			errorFlag = false;
		end
	
		errorUpdateCount = errorUpdateCount + 1;
	end
	
	if whisperFlag == true then
	
		if whispUpdateCount == 4 then
			whispUpdateCount = 0;
			WhisperPixel:SetColorTexture(0, 0, 0, 1);
			whisperFlag = false;
		end
	
		whispUpdateCount = whispUpdateCount + 1;
	end
	
	local shapeShiftForm = GetShapeshiftForm(true);

	ThreeByteOnePixel(shapeShiftForm, ShapeShiftPixel);

	debugString = debugString .. "Shape: " .. shapeShiftForm .. "\n";

	localizedClass, englishClass = UnitClass("player");
	
	local classIndex = 0;
	
	if (localizedClass == "Warrior") then
		debugString = debugString .. CheckGCD("Heroic Strike", debugString);
		classIndex = 1;
	elseif (localizedClass == "Paladin") then
		debugString = debugString .. CheckGCD("Seal of the Crusader", debugString);
		classIndex = 2;
	elseif (localizedClass == "Rogue") then
		debugString = debugString .. CheckGCD("Sinister Strike", debugString);
		classIndex = 3;
	elseif (localizedClass == "Priest") then
		debugString = debugString .. CheckGCD("Smite", debugString);
		classIndex = 4;
	elseif (localizedClass == "Mage") then
		classIndex = 5;
	elseif (localizedClass == "Warlock") then
		classIndex = 6;
	elseif (localizedClass == "Hunter") then
		classIndex = 7;
	elseif (localizedClass == "Shaman") then
		classIndex = 8;
	elseif (localizedClass == "Druid") then
		
		if shapeShiftForm == 0 then
			debugString = debugString .. CheckGCD("Wrath", debugString);
		elseif shapeShiftForm == 1 then
			debugString = debugString .. CheckGCD("Growl", debugString);
		elseif shapeShiftForm == 2 then
			debugString = debugString .. CheckGCD("Claw", debugString);
		elseif shapeShiftForm == 3 then
			debugString = debugString .. CheckGCD("Claw", debugString);
		end
		
		classIndex = 9;
	else
		classIndex = 0;
	end
	
	debugString = debugString .. "Class: " .. classIndex .. "\n";
	ThreeByteOnePixel(classIndex,ClassPixel);

	if playerCasting == true then
		CastingPixel:SetColorTexture(1, 1, 1, 1);
		debugString = debugString .. " Casting: true\n";
	else
		CastingPixel:SetColorTexture(0, 0, 0, 1);
		debugString = debugString .. " Casting: false\n";
	end

	if UnitName("mouseover") == nil then
		MouseOverPixel:SetColorTexture(0, 0, 0, 1);
		debugString = debugString .. "Mouseover: false\n";
	else
		MouseOverPixel:SetColorTexture(1, 1, 1, 1);
		debugString = debugString .. "Mouseover: true\n";
	end
	
	local buffPixelValue = 0;
	for i=1,40 do
	  local name = UnitBuff("player",i)
	  
	  if name == "Seal of the Crusader" then
		buffPixelValue = OR(buffPixelValue, 1)
	  elseif name == "Mark of the Wild" then
		buffPixelValue = OR(buffPixelValue, 2)
	  elseif name == "Thorns" then
		buffPixelValue = OR(buffPixelValue, 4)
	  elseif name == "Seal of Command" then
		buffPixelValue = OR(buffPixelValue, 8)
	  elseif name == "Blessing of Wisdom" then
		buffPixelValue = OR(buffPixelValue, 16)
	  elseif name == "Stealth" then
		buffPixelValue = OR(buffPixelValue, 32)
	  elseif name == "Seal of Justice" then
		buffPixelValue = OR(buffPixelValue, 64)
	  end
	  
	end
	
	ThreeByteOnePixel(buffPixelValue,BuffsPixel);
	
	debugString = debugString .. "Buffs: " .. buffPixelValue .."\n";

	if UnitName("targettarget") == UnitName("player") then
		TargetTargetingPlayerPixel:SetColorTexture(1, 1, 1, 1);
		debugString = debugString .. "Target targeting you: true\n";
	else
		TargetTargetingPlayerPixel:SetColorTexture(0, 0, 0, 1);
		debugString = debugString .. "Target targeting you: false\n";
	end

	local numberOfFreeSlots = 0;

	for i = 0, 4 do
		numberOfFreeSlots = numberOfFreeSlots + GetContainerNumFreeSlots(i);
	end
	
	debugString = debugString .. "Free bag slots: " .. numberOfFreeSlots .. "\n";
	ThreeByteOnePixel(numberOfFreeSlots,BagSlotsFreePixel);

	SetTextDebug(debugString);
end

function VisionControlDebugButton_OnClick()
	if not debugFlag then
		VisionControlDebugButton:SetText("Hide")
	else
		VisionControlDebugButton:SetText("Show")
	end

	debugFlag = not debugFlag;
end

function VisionControlStartButton_OnClick()
	if not startFlag then
		VisionControlStartButton:SetText("Stop")
	else
		VisionControlStartButton:SetText("Start")
	end

	startFlag = not startFlag;
end

local castStopFrame = CreateFrame('Frame');
castStopFrame:RegisterEvent("UNIT_SPELLCAST_STOP")
castStopFrame:SetScript('OnEvent', function(self, event, arg1)
	if arg1 == "player" then
		playerCasting = false;
	end
end)

local errorFrame = CreateFrame('Frame');
errorFrame:RegisterEvent("UI_ERROR_MESSAGE")
errorFrame:SetScript('OnEvent', function(self, event, arg1, arg2)

	if arg2 == "Target needs to be in front of you" then
		ThreeByteOnePixel(1,ErrorPixel);
		errorFlag = true;
	end
	
	if arg2 == "You are facing the wrong way!" then
		ThreeByteOnePixel(2,ErrorPixel);
		errorFlag = true;
	end
	
	if arg2 == "Out of range." then
		ThreeByteOnePixel(3,ErrorPixel);
		errorFlag = true;
	end
	
	if arg2 == "You are too far away!" then
		ThreeByteOnePixel(4,ErrorPixel);
		errorFlag = true;
	end
	
	if arg2 == "Target not in line of sight" then
		ThreeByteOnePixel(5,ErrorPixel);
		errorFlag = true;
	end
	
end)

local castStartFrame = CreateFrame('Frame');
castStartFrame:RegisterEvent("UNIT_SPELLCAST_START")
castStartFrame:SetScript('OnEvent', function(self, event, arg1)
	if arg1 == "player" then
		playerCasting = true;
	end
end)

local whisperFrame = CreateFrame('Frame');
whisperFrame:RegisterEvent("CHAT_MSG_WHISPER")
whisperFrame:SetScript('OnEvent', function(self, event)
	WhisperPixel:SetColorTexture(1, 1, 1, 1);
	whisperFlag = true;
end)

local shellList = {
	["Thick-shelled Clam"] = true,
	["Small Barnacled Clam"] = true,
	["Big-mouth Clam"] = true,
	["Soft-shelled Clam"] = true,
	["Darkwater Clam"] = true,
	["Jaggal Clam"] = true,
	["Abyssal Clam"] = true,
	["Brooding Darkwater Clam"] = true,
	["Giant Darkwater Clam"] = true,
};

local clamFrame = CreateFrame('Frame');
clamFrame:RegisterEvent("BAG_UPDATE")
clamFrame:SetScript('OnEvent', function(self, event)
	if not GetCVarBool("autoLootDefault") then
		return
	end

	for i = 0, 4 do
		local numSlots = GetContainerNumSlots(i);

		for j = 1, numSlots do
			local itemLink = select(7, GetContainerItemInfo(i, j));
		
			if (itemLink) then
				local name = strsub(itemLink, strfind(itemLink, "%[") + 1, strfind(itemLink, "]") - 1);
			
				if (shellList[name]) then
					UseContainerItem(i, j);
				end
			end
		end
	end
end)

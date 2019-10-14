-- Constants

local FLOAT_TO_3_BYTE_INT = 16777215;
local PI = 3.141592654;

-- Globals

local whisperLastUpdate = 0;
local whisperStart = false;
local errorLastUpdate = 0;
local errorStart = false;
local startedAutomater = false;
local showDebug = false;
local isCasting = false;
local keyBindSet = false;

-- Helper functions

local function RightBitShift(a, b)
	return math.floor(a/(2^b));
end

local function BitwiseOR(a,b)
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

local function BitwiseNOT(n)
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

local function BitwiseAND(a,b)
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

local function Round(floatValue)
	local temp = math.floor(floatValue);
	
	if floatValue - temp >= 0.5 then
		return math.floor(floatValue+0.5)
	else
		return temp;
	end
	
end

local function Set2BytePixelValue(pixelValue,Pixel)
	local pixelIntValue = Round(pixelValue);
	local part1 = BitwiseAND(RightBitShift(pixelIntValue,8),255);
	local part2 = BitwiseAND(pixelIntValue,255);
	Pixel:SetColorTexture(part2/255,part1/255,1, 1);
end

local function Set3Byte3PixelValue(pixelValue,PixelR,PixelG,PixelB)
	local pixelIntValue = Round(pixelValue * FLOAT_TO_3_BYTE_INT);
	local part1 = BitwiseAND(RightBitShift(pixelIntValue,16),255);
	local part2 = BitwiseAND(RightBitShift(pixelIntValue,8),255);
	local part3 = BitwiseAND(pixelIntValue,255);

	PixelR:SetColorTexture(part3/255,0,0,1);
	PixelG:SetColorTexture(0,part2/255,0,1);
	PixelB:SetColorTexture(0,0,part1/255,1);
end

local function SetDebugText(debugText, text)
	if showDebug == true then
		debugText:SetText(text);
	else
		debugText:SetText("");
	end
end

local function CheckCanUseSkill(checkName)

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

-- UI Events

function DataFrame_OnUpdate()

	if keyBindSet == false then
		SetBindingClick("SHIFT-T", StartStopButton:GetName());
		keyBindSet = true;
	end

	local debugString = ""

	FindPixel1:SetColorTexture(50/255, 100/255, 150/255, 1);
	FindPixel2:SetColorTexture(150/255, 50/255, 100/255, 1);

	local map = C_Map.GetBestMapForUnit("player")
	
	if map == nil then 
		Set3Byte3PixelValue(0,XPositionR,XPositionG,XPositionB)
		Set3Byte3PixelValue(0,YPositionR,YPositionG,YPositionB)
		Set2BytePixelValue(0,MapIdPixel);
		
		debugString = debugString .. "Map: 0\n";
		debugString = debugString .. "X: 0\n";
		debugString = debugString .. "Y: 0\n";
	else
		local position = C_Map.GetPlayerMapPosition(map, "player")
	
		Set3Byte3PixelValue(position.x,XPositionR,XPositionG,XPositionB)
		Set3Byte3PixelValue(position.y,YPositionR,YPositionG,YPositionB)
		Set2BytePixelValue(map,MapIdPixel);
		
		debugString = debugString .. "Map: " .. map .. "\n";
		debugString = debugString .. "X: " .. position.x*100 .. "\n";
		debugString = debugString .. "Y: " .. position.y*100 .. "\n";
	end
	
	local heading = GetPlayerFacing();
	
	if heading == nil then
		HeadingPixelR:SetColorTexture(0,0,0,1);
		HeadingPixelG:SetColorTexture(0,0,0,1);
		HeadingPixelB:SetColorTexture(0,0,0,1);
		
		Set3Byte3PixelValue(0,HeadingPixelR,HeadingPixelG,HeadingPixelB);
	
		debugString = debugString .. "Heading: 0\n";
	else
		Set3Byte3PixelValue(heading/(2*PI),HeadingPixelR,HeadingPixelG,HeadingPixelB);
	
		debugString = debugString .. "Heading: " .. heading*(180/PI) .. "\n";
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
	Set2BytePixelValue(playerMaxHealth,PlayerMaxHealthPixel);
	
	debugString = debugString .. "Max HP: " .. playerMaxHealth .. "\n";
	
	local playerHealth = UnitHealth("player");
	Set2BytePixelValue(playerHealth,PlayerHealthPixel);
	
	debugString = debugString .. "HP: " .. playerHealth .. "\n";
	
	local playerMaxMana = UnitPowerMax("player");
	Set2BytePixelValue(playerMaxMana,PlayerMaxManaPixel);
	
	debugString = debugString .. "Max MP: " .. playerMaxMana .. "\n";
	
	local playerMana = UnitPower("player");
	Set2BytePixelValue(playerMana,PlayerManaPixel);
	
	debugString = debugString .. "MP: " .. playerMana .. "\n";
	
	if UnitExists("target") then
		HasTargetPixel:SetColorTexture(1, 1, 1, 1);
		debugString = debugString .. "Has Target: True" .. "\n";
	else
		HasTargetPixel:SetColorTexture(0, 0, 0, 1);
		debugString = debugString .. "Has Target: False" .. "\n";
	end
	
	local targetHealth = UnitHealth("target");
	Set2BytePixelValue(targetHealth,TargetHealthPixel);
	
	debugString = debugString .. "Target HP: " .. targetHealth .. "\n";
	
	local targetMana = UnitPower("target");
	Set2BytePixelValue(targetMana,TargetManaPixel);
	
	debugString = debugString .. "Target MP: " .. targetMana .. "\n";
	
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
		Set2BytePixelValue(0,TargetReactionPixel);
		debugString = debugString .. "Reaction: 0" .. "\n";
	else
		Set2BytePixelValue(reaction,TargetReactionPixel);
		debugString = debugString .. "Reaction: " .. reaction .. "\n";
	end
	
	targetLevel = UnitLevel("target");
	Set2BytePixelValue(targetLevel,TargetLevelPixel);
	
	debugString = debugString .. "Target Level: " .. targetLevel .. "\n";
	
	playerLevel = UnitLevel("player");
	Set2BytePixelValue(playerLevel,PlayerLevelPixel);
	
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
		Set2BytePixelValue(0,TargetFactionPixel);
		debugString = debugString .. "Target Faction: 0" .. "\n";
	elseif targetFaction == "Alliance" then
		Set2BytePixelValue(1,TargetFactionPixel);
		debugString = debugString .. "Target Faction: 1" .. "\n";
	elseif targetFaction == "Horde" then
		Set2BytePixelValue(2,TargetFactionPixel);
		debugString = debugString .. "Target Faction: 2" .. "\n";
	end
	
	local comboPoints = GetComboPoints("player", "target");
	Set2BytePixelValue(comboPoints,TargetComboPointsPixel);	
	
	debugString = debugString .. "Combo Points: " .. comboPoints .. "\n";
	
	local ammoCount = GetInventoryItemCount("player", INVSLOT_RANGED );
	Set2BytePixelValue(ammoCount,AmmoCountPixel);	
	
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
	
	if startedAutomater == true then
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
	
	if errorStart == true then
	
		if errorLastUpdate == 4 then
			errorLastUpdate = 0;
			ErrorPixel:SetColorTexture(0, 0, 0, 1);
			errorStart = false;
		end
	
		errorLastUpdate = errorLastUpdate + 1;
	end
	
	if whisperStart == true then
	
		if whisperLastUpdate == 4 then
			whisperLastUpdate = 0;
			WhisperPixel:SetColorTexture(0, 0, 0, 1);
			whisperStart = false;
		end
	
		whisperLastUpdate = whisperLastUpdate + 1;
	end
	
	local shapeShiftForm = GetShapeshiftForm(true);

	Set2BytePixelValue(shapeShiftForm, ShapeShiftPixel);

	debugString = debugString .. "Shape: " .. shapeShiftForm .. "\n";

	localizedClass, englishClass = UnitClass("player");
	
	local classIndex = 0;
	
	if (localizedClass == "Warrior") then
		debugString = debugString .. CheckCanUseSkill("Heroic Strike", debugString);
		classIndex = 1;
	elseif (localizedClass == "Paladin") then
		debugString = debugString .. CheckCanUseSkill("Seal of the Crusader", debugString);
		classIndex = 2;
	elseif (localizedClass == "Rogue") then
		debugString = debugString .. CheckCanUseSkill("Sinister Strike", debugString);
		classIndex = 3;
	elseif (localizedClass == "Priest") then
		debugString = debugString .. CheckCanUseSkill("Smite", debugString);
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
			debugString = debugString .. CheckCanUseSkill("Wrath", debugString);
		elseif shapeShiftForm == 1 then
			debugString = debugString .. CheckCanUseSkill("Growl", debugString);
		elseif shapeShiftForm == 2 then
			debugString = debugString .. CheckCanUseSkill("Claw", debugString);
		elseif shapeShiftForm == 3 then
			debugString = debugString .. CheckCanUseSkill("Claw", debugString);
		end
		
		classIndex = 9;
	else
		classIndex = 0;
	end
	
	debugString = debugString .. "Class: " .. classIndex .. "\n";
	Set2BytePixelValue(classIndex,ClassPixel);

	if isCasting == true then
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
		buffPixelValue = BitwiseOR(buffPixelValue, 1)
	  elseif name == "Mark of the Wild" then
		buffPixelValue = BitwiseOR(buffPixelValue, 2)
	  elseif name == "Thorns" then
		buffPixelValue = BitwiseOR(buffPixelValue, 4)
	  elseif name == "Seal of Command" then
		buffPixelValue = BitwiseOR(buffPixelValue, 8)
	  elseif name == "Blessing of Wisdom" then
		buffPixelValue = BitwiseOR(buffPixelValue, 16)
	  elseif name == "Stealth" then
		buffPixelValue = BitwiseOR(buffPixelValue, 32)
	  end
	  
	end
	
	Set2BytePixelValue(buffPixelValue,BuffsPixel);
	
	debugString = debugString .. "Buffs: " .. buffPixelValue .."\n";

	if UnitName("targettarget") == UnitName("player") then
		TargetTargetingPlayerPixel:SetColorTexture(1, 1, 1, 1);
		debugString = debugString .. "Target targeting you: true\n";
	else
		TargetTargetingPlayerPixel:SetColorTexture(0, 0, 0, 1);
		debugString = debugString .. "Target targeting you: false\n";
	end

	SetDebugText(DebugText,debugString);
end

function StartStopButton_OnClick()
	if not startedAutomater then
		StartStopButton:SetText("Stop")
	else
		StartStopButton:SetText("Start")
	end

	startedAutomater = not startedAutomater;
end

function ShowHideButton_OnClick()
	if not showDebug then
		ShowHideButton:SetText("Hide")
	else
		ShowHideButton:SetText("Show")
	end

	showDebug = not showDebug;
end

function DataFrame_OnLoad()
	
end

-- Error frame event

local errorFrame = CreateFrame('Frame');
errorFrame:RegisterEvent("UI_ERROR_MESSAGE")
errorFrame:SetScript('OnEvent', function(self, event, arg1, arg2)

	if arg2 == "Target needs to be in front of you" then
		Set2BytePixelValue(1,ErrorPixel);
		errorStart = true;
	end
	
	if arg2 == "You are facing the wrong way!" then
		Set2BytePixelValue(2,ErrorPixel);
		errorStart = true;
	end
	
end)

-- Spell cast event

local castStartFrame = CreateFrame('Frame');
castStartFrame:RegisterEvent("UNIT_SPELLCAST_START")
castStartFrame:SetScript('OnEvent', function(self, event, arg1)
	if arg1 == "player" then
		isCasting = true;
	end
end)

-- Spell stop cast event

local castStopFrame = CreateFrame('Frame');
castStopFrame:RegisterEvent("UNIT_SPELLCAST_STOP")
castStopFrame:SetScript('OnEvent', function(self, event, arg1)
	if arg1 == "player" then
		isCasting = false;
	end
end)

-- Whisper event

local whisperFrame = CreateFrame('Frame');
whisperFrame:RegisterEvent("CHAT_MSG_WHISPER")
whisperFrame:SetScript('OnEvent', function(self, event)
	WhisperPixel:SetColorTexture(1, 1, 1, 1);
	whisperStart = true;
end)

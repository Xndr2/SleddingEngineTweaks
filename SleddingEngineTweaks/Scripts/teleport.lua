-- teleport.lua
-- Creates a Teleport panel with Save, Teleport and NoClip toggle

local modName = "Teleport"
local savedPos = nil
local noclip = false
local keySave = "F7"
local keyTeleport = "F8"

local function ensurePanel()
  set.RegisterModPanel(modName)
  set.RegisterModTab(modName, "Controls")
end

local function saveCurrentPosition()
  savedPos = game.GetPlayerPosition()
  if savedPos ~= nil then
    log("Saved position: ("..tostring(savedPos.x)..", "..tostring(savedPos.y)..", "..tostring(savedPos.z)..")")
  else
    log("Failed to get player position")
  end
end

local function teleportToSaved()
  if savedPos ~= nil then
    game.SetPlayerPosition(savedPos)
    log("Teleported to saved position")
  else
    log("No saved position yet")
  end
end

local function toggleNoClip(value)
  noclip = value and true or false
  game.SetNoClip(noclip)
  log("NoClip: "..tostring(noclip))
end

local function setupUI()
  ensurePanel()
  set.RegisterButtonOption(modName, "Controls", "save_btn", "Save Location", function() saveCurrentPosition() end)
  set.RegisterButtonOption(modName, "Controls", "tp_btn", "Teleport", function() teleportToSaved() end)
  set.RegisterSelectorOption(modName, "Controls", "noclip_toggle", "NoClip (fly)", function(v) toggleNoClip(v) end)
end

function OnUpdate(deltaTime)
  -- hotkeys
  if game.WasKeyPressedThisFrame(keySave) then
    saveCurrentPosition()
  end
  if game.WasKeyPressedThisFrame(keyTeleport) then
    teleportToSaved()
  end

  if noclip then
    -- Simple noclip movement using WASD + Space/Ctrl
    local speed = 10.0 * deltaTime
    local move = Vector3(0,0,0)
    if game.IsKeyDown("W") then move = move + Vector3(0,0,1) end
    if game.IsKeyDown("S") then move = move + Vector3(0,0,-1) end
    if game.IsKeyDown("A") then move = move + Vector3(-1,0,0) end
    if game.IsKeyDown("D") then move = move + Vector3(1,0,0) end
    if game.IsKeyDown("Space") then move = move + Vector3(0,1,0) end
    if game.IsKeyDown("LeftCtrl") then move = move + Vector3(0,-1,0) end
    if move.x ~= 0 or move.y ~= 0 or move.z ~= 0 then
      -- Move relative to player local axes
      game.MovePlayerAlong(move, speed)
    end
  end
end

-- init
setupUI()


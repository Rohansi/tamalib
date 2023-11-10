/*
 * TamaLIB - A hardware agnostic Tamagotchi P1 emulation library
 *
 * Copyright (C) 2021 Jean-Christophe Rona <jc@rona.fr>
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */
#include "tamalib.h"

#include <stdlib.h>
#include <string.h>

#include "hw.h"
#include "cpu.h"
#include "hal.h"
#include "rom.h"

LIBRARY_API cpu_t *tamalib_create(
	hal_set_lcd_matrix_handler_t lcd_matrix_handler,
	hal_set_lcd_icon_handler_t lcd_icon_handler,
	hal_set_frequency_handler_t set_frequency_handler,
	hal_play_frequency_handler_t play_frequency_handler)
{
	cpu_t *cpu = malloc(sizeof(cpu_t));
	if (!cpu) return NULL;
	memset(cpu, 0, sizeof(cpu_t));

	cpu_init(cpu, tamagotchi_rom);
	hw_init(cpu);

	cpu->g_hal.set_lcd_matrix = lcd_matrix_handler;
	cpu->g_hal.set_lcd_icon = lcd_icon_handler;
	cpu->g_hal.set_frequency = set_frequency_handler;
	cpu->g_hal.play_frequency = play_frequency_handler;

	return cpu;
}

LIBRARY_API void tamalib_destroy(cpu_t *cpu)
{
	if (!cpu) return;
	hw_release(cpu);
	cpu_release(cpu);
	free(cpu);
}

LIBRARY_API void tamalib_reset(cpu_t *cpu)
{
	if (!cpu) return;
	cpu_reset(cpu);
}

LIBRARY_API void tamalib_set_button(cpu_t *cpu, button_t btn, btn_state_t state)
{
	if (!cpu) return;
	hw_set_button(cpu, btn, state);
}

LIBRARY_API void tamalib_step(cpu_t *cpu, u32_t cycles)
{
	if (!cpu) return;

	u32_t overflow_taken = min(cpu->overflow_cycles, cycles);
	cycles -= overflow_taken;
	cpu->overflow_cycles -= overflow_taken;

	while (cycles > 0) {
		u8_t cycles_ran = cpu_step(cpu);
		if (cycles >= cycles_ran) {
			cycles -= cycles_ran;
		} else {
			cpu->overflow_cycles = cycles_ran - cycles;
			cycles = 0;
		}
	}
}

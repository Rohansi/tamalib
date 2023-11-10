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
#ifndef _TAMALIB_H_
#define _TAMALIB_H_

#include "cpu.h"
#include "hw.h"
#include "hal.h"

#ifdef _WIN32
	#define LIBRARY_API __declspec(dllexport)
#elif
	#define LIBRARY_API
#endif

#define tamalib_refresh_hw(cpu)				cpu_refresh_hw(cpu)

LIBRARY_API cpu_t *tamalib_create(
	hal_set_lcd_matrix_handler_t lcd_matrix_handler,
	hal_set_lcd_icon_handler_t lcd_icon_handler,
	hal_set_frequency_handler_t set_frequency_handler,
	hal_play_frequency_handler_t play_frequency_handler);
LIBRARY_API void tamalib_destroy(cpu_t *cpu);
LIBRARY_API void tamalib_reset(cpu_t *cpu);
LIBRARY_API void tamalib_set_button(cpu_t *cpu, button_t btn, btn_state_t state);
LIBRARY_API void tamalib_step(cpu_t *cpu, u32_t cycles);

#endif /* _TAMALIB_H_ */
